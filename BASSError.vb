﻿Public Enum BassError
    'Error codes returned by BASS_ErrorGetCode
    OK = 0               'all is OK
    MEM = 1        'memory error
    FILEOPEN = 2   'can't open the file
    DRIVER = 3     'can't find a free sound driver
    BUFLOST = 4    'the sample buffer was lost
    HANDLE = 5     'invalid PlaybackStream
    FORMAT = 6     'unsupported sample format
    POSITION = 7   'invalid position
    INIT = 8       'BASS_Init has not been successfully called
    START = 9      'BASS_Start has not been successfully called
    ALREADY = 14   'already initialized/paused/whatever
    NOCHAN = 18    'can't get a free channel
    ILLtype = 19   'an illegalPublic Structure was specified
    ILLPARAM = 20  'an illegal parameter was specified
    NO3D = 21      'no 3D support
    NOEAX = 22     'no EAX support
    DEVICE = 23    'illegal device number
    NOPLAY = 24    'not playing
    FREQ = 25      'illegal sample rate
    NOTFILE = 27   'the stream is not a file stream
    NOHW = 29      'no hardware voices available
    EMPTY = 31     'the MOD music has no sequence data
    NONET = 32     'no internet connection could be opened
    CREATE = 33    'couldn't create the file
    NOFX = 34      'effects are not available
    NOTAVAIL = 37  'requested data is not available
    DECODE = 38    'the channel is a "decoding channel"
    DX = 39        'a sufficient DirectX version is not installed
    TIMEOUT = 40   'connection timedout
    FILEFORM = 41  'unsupported file format
    SPEAKER = 42   'unavailable speaker
    VERSION = 43   'invalid BASS version (used by add-ons)
    CODEC = 44     'codec is not available/supported
    ENDED = 45     'the channel/file has ended
    BUSY = 46      'the device is busy
    UNKNOWN = -1   'some other mystery problem
End Enum
