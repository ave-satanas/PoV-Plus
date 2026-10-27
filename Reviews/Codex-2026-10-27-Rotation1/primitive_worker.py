"""Windows FileShare.None test actor, NOT a resource-lock implementation/production adapter.

Only explicit fixture lock files are supported. No resource identity algorithm,
Valheim/Steam/process discovery, deployment, recovery or global lock store exists.
The parent selects paths to calibrate its oracles; candidate tests remain pending.
"""
import ctypes
from ctypes import wintypes
import json
import os
from pathlib import Path, PureWindowsPath
import re
import stat
import sys
import time
import uuid


def emit(**data):
    print(json.dumps(data), flush=True)


def main():
    if os.name != 'nt':
        raise RuntimeError('Windows-only calibration; do not substitute POSIX semantics')
    repo = Path(__file__).resolve().parents[2]
    expected = repo / '.dev-state/rotation1'
    cursor = Path(expected.anchor)
    for part in expected.parts[1:]:
        cursor /= part
        info = cursor.lstat()
        if stat.S_ISLNK(info.st_mode) or getattr(info, 'st_file_attributes', 0) & 0x400:
            raise RuntimeError('Actor scratch crosses a reparse point')
    expected = expected.resolve(strict=True)
    root = Path(sys.argv[1]).resolve(strict=True)
    if root.parent != expected or not re.fullmatch(r'[a-f0-9]{32}', root.name):
        raise RuntimeError('Actor root must be a direct private run directory in its own worktree')
    cursor = Path(root.anchor)
    for part in root.parts[1:]:
        cursor /= part
        info = cursor.lstat()
        if stat.S_ISLNK(info.st_mode) or getattr(info, 'st_file_attributes', 0) & 0x400:
            raise RuntimeError('Actor root crosses a reparse point')
    marker = json.loads((root / '.rotation1-fixture').read_text())
    if marker.get('kind') != 'synthetic' or not re.fullmatch(r'[a-f0-9]{32}', marker.get('run', '')):
        raise RuntimeError('Not an owned synthetic root')
    kernel = ctypes.WinDLL('kernel32', use_last_error=True)
    kernel.CreateFileW.argtypes = [wintypes.LPCWSTR, wintypes.DWORD, wintypes.DWORD,
        ctypes.c_void_p, wintypes.DWORD, wintypes.DWORD, wintypes.HANDLE]
    kernel.CreateFileW.restype = wintypes.HANDLE
    kernel.CloseHandle.argtypes = [wintypes.HANDLE]
    kernel.CloseHandle.restype = wintypes.BOOL
    invalid = ctypes.c_void_p(-1).value
    handles = {}
    epoch = uuid.uuid4().hex

    def path(relative, *, missing_leaf=True):
        # Harness containment only; NOT the candidate path validator or its oracle.
        if type(relative) is not str or not 0 < len(relative) <= 240:
            raise ValueError('invalid fixture path')
        parts = re.split(r'[\\/]', relative)
        win = PureWindowsPath(relative)
        if win.drive or win.root or any(not x or x in ('.', '..') or
                x.endswith((' ', '.')) or re.search(r'[<>:"|?*\x00-\x1f]', x) or
                x.split('.')[0].upper() in {'CON', 'PRN', 'AUX', 'NUL',
                    *(f'{p}{n}' for p in ('COM', 'LPT') for n in range(1, 10))} for x in parts):
            raise ValueError('unsupported fixture path syntax')
        target = root.joinpath(*parts)
        cursor = Path(target.anchor)
        for part in target.parts[1:]:
            cursor /= part
            try:
                info = cursor.lstat()
            except FileNotFoundError:
                if missing_leaf and cursor == target:
                    continue
                raise ValueError('missing fixture parent') from None
            if stat.S_ISLNK(info.st_mode) or getattr(info, 'st_file_attributes', 0) & 0x400:
                raise ValueError('fixture reparse path')
            if stat.S_ISREG(info.st_mode) and info.st_nlink != 1:
                raise ValueError('fixture hardlink path')
        if not target.resolve().is_relative_to(root):
            raise ValueError('fixture escape')
        return target

    def release(ticket):
        handle = handles.pop(ticket, None)
        if handle is None:
            return False
        if not kernel.CloseHandle(handle):
            raise ctypes.WinError(ctypes.get_last_error())
        return True

    emit(event='ready', pid=os.getpid(), epoch=epoch, run=marker['run'], backend='explicit-file-primitive')
    try:
        for line in sys.stdin:
            request = {}
            try:
                if len(line) > 4096:
                    raise ValueError('request bound')
                request = json.loads(line)
                op = request['op']; identity = request['id']
                if op == 'acquire':
                    target = path(request['path'])
                    timeout = request.get('timeout_ms', 0)
                    if type(timeout) is not int or not 0 <= timeout <= 5000:
                        raise ValueError('timeout bound')
                    cancel = path(request['cancel']) if request.get('cancel') else None
                    deadline = time.monotonic() + timeout / 1000
                    emit(event='attempt', id=identity, pid=os.getpid())
                    while True:
                        if cancel is not None and cancel.exists():
                            emit(event='result', id=identity, status='cancelled'); break
                        handle = kernel.CreateFileW(str(target), 0xC0000000, 0, None, 4, 0x80, None)
                        if handle != invalid:
                            ticket = uuid.uuid4().hex
                            handles[ticket] = handle
                            emit(event='result', id=identity, status='acquired', ticket=ticket,
                                 pid=os.getpid(), epoch=epoch, run=marker['run']); break
                        error = ctypes.get_last_error()
                        if error not in (32, 33):
                            raise ctypes.WinError(error)  # access/identity errors are NOT contention
                        if time.monotonic() >= deadline:
                            emit(event='result', id=identity, status='timeout'); break
                        time.sleep(0.005)  # bounded fixture retry only; no game polling/input
                elif op == 'release':
                    emit(event='result', id=identity, status='released' if release(request['ticket']) else 'not_owner')
                elif op == 'work':
                    ticket = request['ticket']
                    if ticket not in handles:
                        emit(event='result', id=identity, status='not_owner'); continue
                    if request.get('fail'):
                        try:
                            raise RuntimeError('injected fixture exception before side effect')
                        finally:
                            release(ticket)
                    target = path(request['path'])
                    with target.open('x', encoding='utf-8') as stream:
                        stream.write(json.dumps(dict(pid=os.getpid(), epoch=epoch, ticket=ticket)))
                    emit(event='result', id=identity, status='worked')
                elif op == 'crash':
                    emit(event='crashing', id=identity, pid=os.getpid())
                    os._exit(17)  # kills only this deliberately disposable synthetic actor
                elif op == 'quit':
                    emit(event='result', id=identity, status='bye'); return
                else:
                    raise ValueError('unknown operation')
            except Exception as error:
                emit(event='result', id=request.get('id'), status='error', error=type(error).__name__)
    finally:
        for ticket in list(handles):
            release(ticket)


if __name__ == '__main__':
    main()
