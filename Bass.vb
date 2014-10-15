Imports System.Runtime.InteropServices
Imports System.Text

Public Module Bass
    ' BASS 2.4 Visual Basic module
    ' Copyright (c) 1999-2011 Un4seen Developments Ltd.
    '
    ' See the BASS.CHM file for more detailed documentation

    ' NOTE: VB does not support 64-bit integers, so VB users only have access
    '       to the low 32-bits of 64-bit return values. 64-bit parameters can
    '       be specified though, using the "64" version of the function.

    ' NOTE: Use "StrPtr(filename)" to pass a filename to the BASS_MusicLoad,
    '       BASS_SampleLoad and BASS_StreamCreateFile functions.

    ' NOTE: Use the VBStrFromAnsiPtr function to convert "char *" to VB "String".

    Public Const BASSVERSION = &H204    'API version
    Public Const BASSVERSIONTEXT = "2.4"

    Public Const BASSTRUE As Long = 1   'Use this instead of VB Booleans
    Public Const BASSFALSE As Long = 0  'Use this instead of VB Booleans

  
    ' BASS_SetConfig options
    Public Const BASS_CONFIG_BUFFER = 0
    Public Const BASS_CONFIG_UPDATEPERIOD = 1
    Public Const BASS_CONFIG_GVOL_SAMPLE = 4
    Public Const BASS_CONFIG_GVOL_STREAM = 5
    Public Const BASS_CONFIG_GVOL_MUSIC = 6
    Public Const BASS_CONFIG_CURVE_VOL = 7
    Public Const BASS_CONFIG_CURVE_PAN = 8
    Public Const BASS_CONFIG_FLOATDSP = 9
    Public Const BASS_CONFIG_3DALGORITHM = 10
    Public Const BASS_CONFIG_NET_TIMEOUT = 11
    Public Const BASS_CONFIG_NET_BUFFER = 12
    Public Const BASS_CONFIG_PAUSE_NOPLAY = 13
    Public Const BASS_CONFIG_NET_PREBUF = 15
    Public Const BASS_CONFIG_NET_PASSIVE = 18
    Public Const BASS_CONFIG_REC_BUFFER = 19
    Public Const BASS_CONFIG_NET_PLAYLIST = 21
    Public Const BASS_CONFIG_MUSIC_VIRTUAL = 22
    Public Const BASS_CONFIG_VERIFY = 23
    Public Const BASS_CONFIG_UPDATETHREADS = 24
    Public Const BASS_CONFIG_DEV_BUFFER = 27
    Public Const BASS_CONFIG_DEV_DEFAULT = 36
    Public Const BASS_CONFIG_NET_READTIMEOUT = 37

    ' BASS_SetConfigPtr options
    Public Const BASS_CONFIG_NET_AGENT = 16
    Public Const BASS_CONFIG_NET_PROXY = 17

    ' BASS_ASIO_Init flags
    Public Const BASS_DEVICE_8BITS = 1     'use 8 bit resolution, else 16 bit
    Public Const BASS_DEVICE_MONO = 2      'use mono, else stereo
    Public Const BASS_DEVICE_3D = 4        'enable 3D functionality
    Public Const BASS_DEVICE_LATENCY = 256 'calculate device latency (BASS_INFO struct)
    Public Const BASS_DEVICE_CPSPEAKERS = 1024 'detect speakers via Windows control panel
    Public Const BASS_DEVICE_SPEAKERS = 2048 'force enabling of speaker assignment
    Public Const BASS_DEVICE_NOSPEAKER = 4096 'ignore speaker arrangement

    ' DirectSound interfaces (for use with BASS_GetDSoundObject)
    Public Const BASS_OBJECT_DS = 1                     ' DirectSound
    Public Const BASS_OBJECT_DS3DL = 2                  'IDirectSound3DListener

    ' Device info structure
    Public Structure BASS_DEVICEINFO
        Public name As Long          ' description
        Public driver As Long        ' driver
        Public flags As Long
    End Structure

    ' BASS_DEVICEINFO flags
    Public Const BASS_DEVICE_ENABLED = 1
    Public Const BASS_DEVICE_DEFAULT = 2
    Public Const BASS_DEVICE_INIT = 4

    Public Structure BASS_INFO
        Public flags As Long         ' device capabilities (DSCAPS_xxx flags)
        Public hwsize As Long        ' size of total device hardware memory
        Public hwfree As Long        ' size of free device hardware memory
        Public freesam As Long       ' number of free sample slots in the hardware
        Public free3d As Long        ' number of free 3D sample slots in the hardware
        Public minrate As Long       ' min sample rate supported by the hardware
        Public maxrate As Long       ' max sample rate supported by the hardware
        Public eax As Long           ' device supports EAX? (always BASSFALSE if BASS_DEVICE_3D was not used)
        Public minbuf As Long        ' recommended minimum buffer length in ms (requires BASS_DEVICE_LATENCY)
        Public dsver As Long         ' DirectSound version
        Public latency As Long       ' delay (in ms) before start of playback (requires BASS_DEVICE_LATENCY)
        Public initflags As Long     ' BASS_Init "flags" parameter
        Public speakers As Long      ' number of speakers available
        Public freq As Long          ' current output rate (OSX only)
    End Structure

    ' BASS_INFO flags (from DSOUND.H)
    Public Const DSCAPS_CONTINUOUSRATE = 16    ' supports all sample rates between min/maxrate
    Public Const DSCAPS_EMULDRIVER = 32        ' device does NOT have hardware DirectSound support
    Public Const DSCAPS_CERTIFIED = 64         ' device driver has been certified by Microsoft
    Public Const DSCAPS_SECONDARYMONO = 256    ' mono
    Public Const DSCAPS_SECONDARYSTEREO = 512  ' stereo
    Public Const DSCAPS_SECONDARY8BIT = 1024   ' 8 bit
    Public Const DSCAPS_SECONDARY16BIT = 2048  ' 16 bit

    ' Recording device info structure
    Public Structure BassRecordinfo
        Public Flags As Long         ' device capabilities (DSCCAPS_xxx flags)
        Public Formats As Long       ' supported standard formats (WAVE_FORMAT_xxx flags)
        Public Inputs As Long        ' number of inputs
        Public Singlein As Long      ' BASSTRUE = only 1 input can be set at a time
        Public Freq As Long          ' current input rate (Vista/OSX only)
    End Structure

    ' BASS_RECORDINFO flags (from DSOUND.H)
    Public Const DSCCAPS_EMULDRIVER = DSCAPS_EMULDRIVER ' device does NOT have hardware DirectSound recording support
    Public Const DSCCAPS_CERTIFIED = DSCAPS_CERTIFIED   ' device driver has been certified by Microsoft

    ' defines for formats field of BASS_RECORDINFO (from MMSYSTEM.H)
    Public Const WAVE_FORMAT_1M08 = &H1          ' 11.025 kHz, Mono,   8-bit
    Public Const WAVE_FORMAT_1S08 = &H2          ' 11.025 kHz, Stereo, 8-bit
    Public Const WAVE_FORMAT_1M16 = &H4          ' 11.025 kHz, Mono,   16-bit
    Public Const WAVE_FORMAT_1S16 = &H8          ' 11.025 kHz, Stereo, 16-bit
    Public Const WAVE_FORMAT_2M08 = &H10         ' 22.05  kHz, Mono,   8-bit
    Public Const WAVE_FORMAT_2S08 = &H20         ' 22.05  kHz, Stereo, 8-bit
    Public Const WAVE_FORMAT_2M16 = &H40         ' 22.05  kHz, Mono,   16-bit
    Public Const WAVE_FORMAT_2S16 = &H80         ' 22.05  kHz, Stereo, 16-bit
    Public Const WAVE_FORMAT_4M08 = &H100        ' 44.1   kHz, Mono,   8-bit
    Public Const WAVE_FORMAT_4S08 = &H200        ' 44.1   kHz, Stereo, 8-bit
    Public Const WAVE_FORMAT_4M16 = &H400        ' 44.1   kHz, Mono,   16-bit
    Public Const WAVE_FORMAT_4S16 = &H800        ' 44.1   kHz, Stereo, 16-bit

    ' Sample info structure
    Public Structure BassSample
        Public freq As Long          ' default playback rate
        Public volume As Single      ' default volume (0-100)
        Public pan As Single         ' default pan (-100=left, 0=middle, 100=right)
        Public flags As Long         ' BASS_SAMPLE_xxx flags
        Public length As Long        ' length (in samples, not bytes)
        Public max As Long           ' maximum simultaneous playbacks
        Public origres As Long       ' original resolution
        Public chans As Long         ' number of channels
        Public mingap As Long        ' minimum gap (ms) between creating channels
        Public mode3d As Long        ' BASS_3DMODE_xxx mode
        Public mindist As Single     ' minimum distance
        Public MAXDIST As Single     ' maximum distance
        Public iangle As Long        ' angle of inside projection cone
        Public oangle As Long        ' angle of outside projection cone
        Public outvol As Single      ' delta-volume outside the projection cone
        Public vam As Long           ' voice allocation/management flags (BASS_VAM_xxx)
        Public priority As Long      ' priority (0=lowest, &Hffffffff=highest)
    End Structure

    Public Const BASS_SAMPLE_8BITS = 1          ' 8 bit
    Public Const BASS_SAMPLE_FLOAT = 256        ' 32-bit floating-point
    Public Const BASS_SAMPLE_MONO = 2           ' mono
    Public Const BASS_SAMPLE_LOOP = 4           ' looped
    Public Const BASS_SAMPLE_3D = 8             ' 3D functionality
    Public Const BASS_SAMPLE_SOFTWARE = 16      ' not using hardware mixing
    Public Const BASS_SAMPLE_MUTEMAX = 32       ' mute at max distance (3D only)
    Public Const BASS_SAMPLE_VAM = 64           ' DX7 voice allocation & management
    Public Const BASS_SAMPLE_FX = 128           ' old implementation of DX8 effects
    Public Const BASS_SAMPLE_OVER_VOL = &H10000 ' override lowest volume
    Public Const BASS_SAMPLE_OVER_POS = &H20000 ' override longest playing
    Public Const BASS_SAMPLE_OVER_DIST = &H30000 ' override furthest from listener (3D only)

    Public Const BASS_STREAM_PRESCAN = &H20000   ' enable pin-point seeking/length (MP3/MP2/MP1)
    Public Const BASS_MP3_SETPOS = BASS_STREAM_PRESCAN
    Public Const BASS_STREAM_AUTOFREE = &H40000 ' automatically free the stream when it stop/ends
    Public Const BASS_STREAM_RESTRATE = &H80000 ' restrict the download rate of internet file streams
    Public Const BASS_STREAM_BLOCK = &H100000   ' download/play internet file stream in small blocks
    Public Const BASS_STREAM_DECODE = &H200000  ' don't play the stream, only decode (BASS_ChannelGetData)
    Public Const BASS_STREAM_STATUS = &H800000  ' give server status info (HTTP/ICY tags) in DOWNLOADPROC

    Public Const BASS_MUSIC_FLOAT = BASS_SAMPLE_FLOAT
    Public Const BASS_MUSIC_MONO = BASS_SAMPLE_MONO
    Public Const BASS_MUSIC_LOOP = BASS_SAMPLE_LOOP
    Public Const BASS_MUSIC_3D = BASS_SAMPLE_3D
    Public Const BASS_MUSIC_FX = BASS_SAMPLE_FX
    Public Const BASS_MUSIC_AUTOFREE = BASS_STREAM_AUTOFREE
    Public Const BASS_MUSIC_DECODE = BASS_STREAM_DECODE
    Public Const BASS_MUSIC_PRESCAN = BASS_STREAM_PRESCAN ' calculate playback length
    Public Const BASS_MUSIC_CALCLEN = BASS_MUSIC_PRESCAN
    Public Const BASS_MUSIC_RAMP = &H200        ' normal ramping
    Public Const BASS_MUSIC_RAMPS = &H400       ' sensitive ramping
    Public Const BASS_MUSIC_SURROUND = &H800    ' surround sound
    Public Const BASS_MUSIC_SURROUND2 = &H1000  ' surround sound (mode 2)
    Public Const BASS_MUSIC_FT2MOD = &H2000     ' play .MOD as FastTracker 2 does
    Public Const BASS_MUSIC_PT1MOD = &H4000     ' play .MOD as ProTracker 1 does
    Public Const BASS_MUSIC_NONINTER = &H10000  ' non-interpolated sample mixing
    Public Const BASS_MUSIC_SINCINTER = &H800000 ' sinc interpolated sample mixing
    Public Const BASS_MUSIC_POSRESET = 32768  ' stop all notes when moving position
    Public Const BASS_MUSIC_POSRESETEX = &H400000 ' stop all notes and reset bmp/etc when moving position
    Public Const BASS_MUSIC_STOPBACK = &H80000  ' stop the music on a backwards jump effect
    Public Const BASS_MUSIC_NOSAMPLE = &H100000 ' don't load the samples

    ' Speaker assignment flags
    Public Const BASS_SPEAKER_FRONT = &H1000000 ' front speakers
    Public Const BASS_SPEAKER_REAR = &H2000000  ' rear/side speakers
    Public Const BASS_SPEAKER_CENLFE = &H3000000 ' center & LFE speakers (5.1)
    Public Const BASS_SPEAKER_REAR2 = &H4000000 ' rear center speakers (7.1)
    Public Const BASS_SPEAKER_LEFT = &H10000000 ' modifier: left
    Public Const BASS_SPEAKER_RIGHT = &H20000000 ' modifier: right
    Public Const BASS_SPEAKER_FRONTLEFT = BASS_SPEAKER_FRONT Or BASS_SPEAKER_LEFT
    Public Const BASS_SPEAKER_FRONTRIGHT = BASS_SPEAKER_FRONT Or BASS_SPEAKER_RIGHT
    Public Const BASS_SPEAKER_REARLEFT = BASS_SPEAKER_REAR Or BASS_SPEAKER_LEFT
    Public Const BASS_SPEAKER_REARRIGHT = BASS_SPEAKER_REAR Or BASS_SPEAKER_RIGHT
    Public Const BASS_SPEAKER_CENTER = BASS_SPEAKER_CENLFE Or BASS_SPEAKER_LEFT
    Public Const BASS_SPEAKER_LFE = BASS_SPEAKER_CENLFE Or BASS_SPEAKER_RIGHT
    Public Const BASS_SPEAKER_REAR2LEFT = BASS_SPEAKER_REAR2 Or BASS_SPEAKER_LEFT
    Public Const BASS_SPEAKER_REAR2RIGHT = BASS_SPEAKER_REAR2 Or BASS_SPEAKER_RIGHT

    Public Const BASS_UNICODE = &H80000000

    Public Const BASS_RECORD_PAUSE = 32768 ' start recording paused

    ' DX7 voice allocation flags
    Public Const BASS_VAM_HARDWARE = 1
    Public Const BASS_VAM_SOFTWARE = 2
    Public Const BASS_VAM_TERM_TIME = 4
    Public Const BASS_VAM_TERM_DIST = 8
    Public Const BASS_VAM_TERM_PRIO = 16

    ' Channel info structure
    Public Structure BASS_CHANNELINFO
        Public freq As Long          ' default playback rate
        Public chans As Long         ' channels
        Public flags As Long         ' BASS_SAMPLE/STREAM/MUSIC/SPEAKER flags
        Public [cType] As Long         'type of channel
        Public origres As Long       ' original resolution
        Public plugin As Long        ' plugin
        Public sample As Long        ' sample
        Public filename As Long      ' filename
    End Structure

    ' BASS_CHANNELINFO types
    Public Const BASS_CTYPE_SAMPLE = 1
    Public Const BASS_CTYPE_RECORD = 2
    Public Const BASS_CTYPE_STREAM = &H10000
    Public Const BASS_CTYPE_STREAM_OGG = &H10002
    Public Const BASS_CTYPE_STREAM_MP1 = &H10003
    Public Const BASS_CTYPE_STREAM_MP2 = &H10004
    Public Const BASS_CTYPE_STREAM_MP3 = &H10005
    Public Const BASS_CTYPE_STREAM_AIFF = &H10006
    Public Const BASS_CTYPE_STREAM_WAV = &H40000 ' WAVE flag, LOWORD=codec
    Public Const BASS_CTYPE_STREAM_WAV_PCM = &H50001
    Public Const BASS_CTYPE_STREAM_WAV_FLOAT = &H50003
    Public Const BASS_CTYPE_MUSIC_MOD = &H20000
    Public Const BASS_CTYPE_MUSIC_MTM = &H20001
    Public Const BASS_CTYPE_MUSIC_S3M = &H20002
    Public Const BASS_CTYPE_MUSIC_XM = &H20003
    Public Const BASS_CTYPE_MUSIC_IT = &H20004
    Public Const BASS_CTYPE_MUSIC_MO3 = &H100    ' MO3 flag

    Public Structure BASS_PLUGINFORM
        Public [ctype] As Long         ' channel type
        Public name As Long          ' format description
        Public exts As Long          ' file extension filter (*.ext1;*.ext2;etc...)
    End Structure

    Public Structure BASS_PLUGININFO
        Public Version As Long       ' version (same form as BASS_GetVersion)
        Public formatc As Long       ' number of formats
        Public formats As Long       ' the array of formats
    End Structure

    ' 3D vector (for 3D positions/velocities/orientations)
    Public Structure BASS_3DVECTOR
        Public X As Single           ' +=right, -=left
        Public Y As Single           ' +=up, -=down
        Public z As Single           ' +=front, -=behind
    End Structure

    ' 3D channel modes
    Public Const BASS_3DMODE_NORMAL = 0     ' normal 3D processing
    Public Const BASS_3DMODE_RELATIVE = 1   ' position is relative to the listener
    Public Const BASS_3DMODE_OFF = 2        ' no 3D processing

    ' software 3D mixing algorithms (used with BASS_CONFIG_3DALGORITHM)
    Public Const BASS_3DALG_DEFAULT = 0
    Public Const BASS_3DALG_OFF = 1
    Public Const BASS_3DALG_FULL = 2
    Public Const BASS_3DALG_LIGHT = 3

    ' EAX environments, use with BASS_SetEAXParameters
    Public Const EAX_ENVIRONMENT_GENERIC = 0
    Public Const EAX_ENVIRONMENT_PADDEDCELL = 1
    Public Const EAX_ENVIRONMENT_ROOM = 2
    Public Const EAX_ENVIRONMENT_BATHROOM = 3
    Public Const EAX_ENVIRONMENT_LIVINGROOM = 4
    Public Const EAX_ENVIRONMENT_STONEROOM = 5
    Public Const EAX_ENVIRONMENT_AUDITORIUM = 6
    Public Const EAX_ENVIRONMENT_CONCERTHALL = 7
    Public Const EAX_ENVIRONMENT_CAVE = 8
    Public Const EAX_ENVIRONMENT_ARENA = 9
    Public Const EAX_ENVIRONMENT_HANGAR = 10
    Public Const EAX_ENVIRONMENT_CARPETEDHALLWAY = 11
    Public Const EAX_ENVIRONMENT_HALLWAY = 12
    Public Const EAX_ENVIRONMENT_STONECORRIDOR = 13
    Public Const EAX_ENVIRONMENT_ALLEY = 14
    Public Const EAX_ENVIRONMENT_FOREST = 15
    Public Const EAX_ENVIRONMENT_CITY = 16
    Public Const EAX_ENVIRONMENT_MOUNTAINS = 17
    Public Const EAX_ENVIRONMENT_QUARRY = 18
    Public Const EAX_ENVIRONMENT_PLAIN = 19
    Public Const EAX_ENVIRONMENT_PARKINGLOT = 20
    Public Const EAX_ENVIRONMENT_SEWERPIPE = 21
    Public Const EAX_ENVIRONMENT_UNDERWATER = 22
    Public Const EAX_ENVIRONMENT_DRUGGED = 23
    Public Const EAX_ENVIRONMENT_DIZZY = 24
    Public Const EAX_ENVIRONMENT_PSYCHOTIC = 25
    Public Const EAX_ENVIRONMENT_COUNT = 26 ' total number of environments

    Public Const BASS_STREAMPROC_END = &H80000000 ' end of user stream flag

    ' special STREAMPROCs
    Public Const STREAMPROC_DUMMY = 0 ' "dummy" stream
    Public Const STREAMPROC_PUSH = -1 ' push stream

    ' BASS_StreamCreateFileUser file systems
    Public Const STREAMFILE_NOBUFFER = 0
    Public Const STREAMFILE_BUFFER = 1
    Public Const STREAMFILE_BUFFERPUSH = 2


    ' BASS_StreamPutFileData options
    Public Const BASS_FILEDATA_END = 0 ' end & close the file

    ' BASS_StreamGetFilePosition modes
    Public Const BASS_FILEPOS_CURRENT = 0
    Public Const BASS_FILEPOS_DECODE = BASS_FILEPOS_CURRENT
    Public Const BASS_FILEPOS_DOWNLOAD = 1
    Public Const BASS_FILEPOS_END = 2
    Public Const BASS_FILEPOS_START = 3
    Public Const BASS_FILEPOS_CONNECTED = 4
    Public Const BASS_FILEPOS_BUFFER = 5

    ' BASS_ChannelSetSync types
    Public Const BASS_SYNC_POS = 0
    Public Const BASS_SYNC_END = 2
    Public Const BASS_SYNC_META = 4
    Public Const BASS_SYNC_SLIDE = 5
    Public Const BASS_SYNC_STALL = 6
    Public Const BASS_SYNC_DOWNLOAD = 7
    Public Const BASS_SYNC_FREE = 8
    Public Const BASS_SYNC_SETPOS = 11
    Public Const BASS_SYNC_MUSICPOS = 10
    Public Const BASS_SYNC_MUSICINST = 1
    Public Const BASS_SYNC_MUSICFX = 3
    Public Const BASS_SYNC_OGG_CHANGE = 12
    Public Const BASS_SYNC_MIXTIME = &H40000000 ' FLAG: sync at mixtime, else at playtime
    Public Const BASS_SYNC_ONETIME = &H80000000 ' FLAG: sync only once, else continuously

    ' BASS_ChannelIsActive return values
    Public Const BASS_ACTIVE_STOPPED = 0
    Public Const BASS_ACTIVE_PLAYING = 1
    Public Const BASS_ACTIVE_STALLED = 2
    Public Const BASS_ACTIVE_PAUSED = 3

    ' Channel attributes
    Public Const BASS_ATTRIB_FREQ = 1
    Public Const BASS_ATTRIB_VOL = 2
    Public Const BASS_ATTRIB_PAN = 3
    Public Const BASS_ATTRIB_EAXMIX = 4
    Public Const BASS_ATTRIB_NOBUFFER = 5
    Public Const BASS_ATTRIB_CPU = 7
    Public Const BASS_ATTRIB_MUSIC_AMPLIFY = &H100
    Public Const BASS_ATTRIB_MUSIC_PANSEP = &H101
    Public Const BASS_ATTRIB_MUSIC_PSCALER = &H102
    Public Const BASS_ATTRIB_MUSIC_BPM = &H103
    Public Const BASS_ATTRIB_MUSIC_SPEED = &H104
    Public Const BASS_ATTRIB_MUSIC_VOL_GLOBAL = &H105
    Public Const BASS_ATTRIB_MUSIC_VOL_CHAN = &H200 ' + channel #
    Public Const BASS_ATTRIB_MUSIC_VOL_INST = &H300 ' + instrument #

    ' BASS_ChannelGetData flags
    Public Const BASS_DATA_AVAILABLE = 0         ' query how much data is buffered
    Public Const BASS_DATA_FLOAT = &H40000000    ' flag: return floating-point sample data
    Public Const BASS_DATA_FFT256 = &H80000000   ' 256 sample FFT
    Public Const BASS_DATA_FFT512 = &H80000001   ' 512 FFT
    Public Const BASS_DATA_FFT1024 = &H80000002  ' 1024 FFT
    Public Const BASS_DATA_FFT2048 = &H80000003  ' 2048 FFT
    Public Const BASS_DATA_FFT4096 = &H80000004  ' 4096 FFT
    Public Const BASS_DATA_FFT8192 = &H80000005  ' 8192 FFT
    Public Const BASS_DATA_FFT16384 = &H80000006 ' 16384 FFT
    Public Const BASS_DATA_FFT_INDIVIDUAL = &H10 ' FFT flag: FFT for each channel, else all combined
    Public Const BASS_DATA_FFT_NOWINDOW = &H20   ' FFT flag: no Hanning window
    Public Const BASS_DATA_FFT_REMOVEDC = &H40   ' FFT flag: pre-remove DC bias

    ' BASS_ChannelGetTags types : what's returned
    Public Const BASS_TAG_ID3 = 0                ' ID3v1 tags : TAG_ID3 structure
    Public Const BASS_TAG_ID3V2 = 1              ' ID3v2 tags : variable length block
    Public Const BASS_TAG_OGG = 2                ' OGG comments : series of null-terminated UTF-8 strings
    Public Const BASS_TAG_HTTP = 3               ' HTTP headers : series of null-terminated ANSI strings
    Public Const BASS_TAG_ICY = 4                ' ICY headers : series of null-terminated ANSI strings
    Public Const BASS_TAG_META = 5               ' ICY metadata : ANSI string
    Public Const BASS_TAG_APE = 6                ' APEv2 tags : series of null-terminated UTF-8 strings
    Public Const BASS_TAG_MP4 = 7                ' MP4/iTunes metadata : series of null-terminated UTF-8 strings
    Public Const BASS_TAG_VENDOR = 9             ' OGG encoder : UTF-8 string
    Public Const BASS_TAG_LYRICS3 = 10           ' Lyric3v2 tag : ASCII string
    Public Const BASS_TAG_CA_CODEC = 11          ' CoreAudio codec info : TAG_CA_CODEC structure
    Public Const BASS_TAG_MF = 13                ' Media Foundation tags : series of null-terminated UTF-8 strings
    Public Const BASS_TAG_WAVEFORMAT = 14        ' WAVE format : WAVEFORMATEEX structure
    Public Const BASS_TAG_RIFF_INFO = &H100      ' RIFF "INFO" tags : series of null-terminated ANSI strings
    Public Const BASS_TAG_RIFF_BEXT = &H101      ' RIFF/BWF "bext" tags : TAG_BEXT structure
    Public Const BASS_TAG_RIFF_CART = &H102      ' RIFF/BWF "cart" tags : TAG_CART structure
    Public Const BASS_TAG_RIFF_DISP = &H103      ' RIFF "DISP" text tag : ANSI string
    Public Const BASS_TAG_APE_BINARY = &H1000    ' + index #, binary APEv2 tag : TAG_APE_BINARY structure
    Public Const BASS_TAG_MUSIC_NAME = &H10000   ' MOD music name : ANSI string
    Public Const BASS_TAG_MUSIC_ORDERS = &H10002 ' MOD order list : BYTE array of pattern numbers
    Public Const BASS_TAG_MUSIC_MESSAGE = &H10001 ' MOD message : ANSI string
    Public Const BASS_TAG_MUSIC_INST = &H10100   ' + instrument #, MOD instrument name : ANSI string
    Public Const BASS_TAG_MUSIC_SAMPLE = &H10300 ' + sample #, MOD sample name : ANSI string

    ' ID3v1 tag structure
    Public Structure TAG_ID3
        Public id As String '* 3
        Public title As String ' * 30
        Public artist As String '* 30
        Public album As String '* 30
        Public year As String '* 4
        Public comment As String '* 30
        Public genre As Byte
    End Structure

    ' Binary APEv2 tag structure
    Public Structure TAG_APE_BINARY
        Public key As Long
        Public data As Long
        Public length As Long
    End Structure

    ' BWF "bext" tag structure
    Public Structure TAG_BEXT
        Public Description As String '* 256         ' description
        Public Originator As String '* 32           ' name of the originator
        Public OriginatorReference As String '* 32  ' reference of the originator
        Public OriginationDate As String '* 10      ' date of creation (yyyy-mm-dd)
        Public OriginationTime As String '* 8       ' time of creation (hh-mm-ss)
        Public TimeReferenceLo As Long             ' low 32 bits of first sample count since midnight (little-endian)
        Public TimeReferenceHi As Long             ' high 32 bits of first sample count since midnight (little-endian)
        Public Version As Integer                  ' BWF version (little-endian)
        Public UMID As Byte() '(0 To 63) As Byte               ' SMPTE UMID
        Public Reserved() As Byte '(0 To 189) As Byte
        Public CodingHistory() As String           ' history
    End Structure

    ' BASS_ChannelGetLength/GetPosition/SetPosition modes
    Public Const BASS_POS_BYTE = 0          ' byte position
    Public Const BASS_POS_MUSIC_ORDER = 1   ' order.row position, MAKELONG(order,row)
    Public Const BASS_POS_DECODE = &H10000000 ' flag: get the decoding (not playing) position
    Public Const BASS_POS_DECODETO = &H20000000 ' flag: decode to the position instead of seeking

    ' BASS_RecordSetInput flags
    Public Const BASS_INPUT_OFF = &H10000
    Public Const BASS_INPUT_ON = &H20000

    Public Const BASS_INPUT_TYPE_MASK = &HFF000000
    Public Const BASS_INPUT_TYPE_UNDEF = &H0
    Public Const BASS_INPUT_TYPE_DIGITAL = &H1000000
    Public Const BASS_INPUT_TYPE_LINE = &H2000000
    Public Const BASS_INPUT_TYPE_MIC = &H3000000
    Public Const BASS_INPUT_TYPE_SYNTH = &H4000000
    Public Const BASS_INPUT_TYPE_CD = &H5000000
    Public Const BASS_INPUT_TYPE_PHONE = &H6000000
    Public Const BASS_INPUT_TYPE_SPEAKER = &H7000000
    Public Const BASS_INPUT_TYPE_WAVE = &H8000000
    Public Const BASS_INPUT_TYPE_AUX = &H9000000
    Public Const BASS_INPUT_TYPE_ANALOG = &HA000000

    ' DX8 effect types, use with BASS_ChannelSetFX
    Public Const BASS_FX_DX8_CHORUS = 0
    Public Const BASS_FX_DX8_COMPRESSOR = 1
    Public Const BASS_FX_DX8_DISTORTION = 2
    Public Const BASS_FX_DX8_ECHO = 3
    Public Const BASS_FX_DX8_FLANGER = 4
    Public Const BASS_FX_DX8_GARGLE = 5
    Public Const BASS_FX_DX8_I3DL2REVERB = 6
    Public Const BASS_FX_DX8_PARAMEQ = 7
    Public Const BASS_FX_DX8_REVERB = 8

    Public Structure BASS_DX8_CHORUS
        Public fWetDryMix As Single
        Public fDepth As Single
        Public fFeedback As Single
        Public fFrequency As Single
        Public lWaveform As Long   ' 0=triangle, 1=sine
        Public fDelay As Single
        Public lPhase As Long              ' BASS_DX8_PHASE_xxx
    End Structure

    Public Structure BASS_DX8_COMPRESSOR
        Public fGain As Single
        Public fAttack As Single
        Public fRelease As Single
        Public fThreshold As Single
        Public fRatio As Single
        Public fPredelay As Single
    End Structure

    Public Structure BASS_DX8_DISTORTION
        Public fGain As Single
        Public fEdge As Single
        Public fPostEQCenterFrequency As Single
        Public fPostEQBandwidth As Single
        Public fPreLowpassCutoff As Single
    End Structure

    Public Structure BASS_DX8_ECHO
        Public fWetDryMix As Single
        Public fFeedback As Single
        Public fLeftDelay As Single
        Public fRightDelay As Single
        Public lPanDelay As Long
    End Structure

    Public Structure BASS_DX8_FLANGER
        Public fWetDryMix As Single
        Public fDepth As Single
        Public fFeedback As Single
        Public fFrequency As Single
        Public lWaveform As Long   ' 0=triangle, 1=sine
        Public fDelay As Single
        Public lPhase As Long              ' BASS_DX8_PHASE_xxx
    End Structure

    Public Structure BASS_DX8_GARGLE
        Public dwRateHz As Long               ' Rate of modulation in hz
        Public dwWaveShape As Long            ' 0=triangle, 1=square
    End Structure

    Public Structure BASS_DX8_I3DL2REVERB
        Public lRoom As Long                    ' [-10000, 0]      default: -1000 mB
        Public lRoomHF As Long                  ' [-10000, 0]      default: 0 mB
        Public flRoomRolloffFactor As Single    ' [0.0, 10.0]      default: 0.0
        Public flDecayTime As Single            ' [0.1, 20.0]      default: 1.49s
        Public flDecayHFRatio As Single         ' [0.1, 2.0]       default: 0.83
        Public lReflections As Long             ' [-10000, 1000]   default: -2602 mB
        Public flReflectionsDelay As Single     ' [0.0, 0.3]       default: 0.007 s
        Public lReverb As Long                  ' [-10000, 2000]   default: 200 mB
        Public flReverbDelay As Single          ' [0.0, 0.1]       default: 0.011 s
        Public flDiffusion As Single            ' [0.0, 100.0]     default: 100.0 %
        Public flDensity As Single              ' [0.0, 100.0]     default: 100.0 %
        Public flHFReference As Single          ' [20.0, 20000.0]  default: 5000.0 Hz
    End Structure

    Public Structure BASS_DX8_PARAMEQ
        Public fCenter As Single
        Public fBandwidth As Single
        Public fGain As Single
    End Structure

    Public Structure BASS_DX8_REVERB
        Public fInGain As Single                ' [-96.0,0.0]            default: 0.0 dB
        Public fReverbMix As Single             ' [-96.0,0.0]            default: 0.0 db
        Public fReverbTime As Single            ' [0.001,3000.0]         default: 1000.0 ms
        Public fHighFreqRTRatio As Single       ' [0.001,0.999]          default: 0.001
    End Structure

    Public Const BASS_DX8_PHASE_NEG_180 = 0
    Public Const BASS_DX8_PHASE_NEG_90 = 1
    Public Const BASS_DX8_PHASE_ZERO = 2
    Public Const BASS_DX8_PHASE_90 = 3
    Public Const BASS_DX8_PHASE_180 = 4

    Public Structure GUID       ' used with BASS_Init - use VarPtr(guid) in clsid parameter
        Dim Data1 As Long
        Dim Data2 As Integer
        Dim Data3 As Integer
        Dim Data4 As Byte() '(0 To 7) As Byte
    End Structure

    Public Enum BassConfigOptions As Short
        ' BASS_Set/GetConfig options
        BASS_CONFIG_BUFFER = 0
        BASS_CONFIG_UPDATEPERIOD = 1
        BASS_CONFIG_MAXVOL = 3
        BASS_CONFIG_GVOL_SAMPLE = 4
        BASS_CONFIG_GVOL_STREAM = 5
        BASS_CONFIG_GVOL_MUSIC = 6
        BASS_CONFIG_CURVE_VOL = 7
        BASS_CONFIG_CURVE_PAN = 8
        BASS_CONFIG_FLOATDSP = 9
        BASS_CONFIG_3DALGORITHM = 10
        BASS_CONFIG_NET_TIMEOUT = 11
        BASS_CONFIG_NET_BUFFER = 12
    End Enum    ' BassConfigOptions

#Region "Config"
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_SetConfig([option] As BASSConfig, <[In], MarshalAs(UnmanagedType.Bool)> newvalue As Boolean) As Boolean
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_SetConfig([option] As BASSConfig, newvalue As Integer) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_SetConfigPtr([option] As BASSConfig, newvalue As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    Public Enum BASSConfig
        BASS_CONFIG_3DALGORITHM = 10
        BASS_CONFIG_AAC_MP4 = &H10701
        BASS_CONFIG_AC3_DYNRNG = &H10001
        BASS_CONFIG_ASYNCFILE_BUFFER = &H2D
        BASS_CONFIG_BUFFER = 0
        BASS_CONFIG_CD_AUTOSPEED = &H10202
        BASS_CONFIG_CD_CDDB_SERVER = &H10204
        BASS_CONFIG_CD_FREEOLD = &H10200
        BASS_CONFIG_CD_RETRY = &H10201
        BASS_CONFIG_CD_SKIPERROR = &H10203
        BASS_CONFIG_CURVE_PAN = 8
        BASS_CONFIG_CURVE_VOL = 7
        BASS_CONFIG_DEV_BUFFER = &H1B
        BASS_CONFIG_DEV_DEFAULT = &H24
        BASS_CONFIG_ENCODE_ACM_LOAD = &H10302
        BASS_CONFIG_ENCODE_CAST_PROXY = &H10311
        BASS_CONFIG_ENCODE_CAST_TIMEOUT = &H10310
        BASS_CONFIG_ENCODE_PRIORITY = &H10300
        BASS_CONFIG_ENCODE_QUEUE = &H10301
        BASS_CONFIG_FLOATDSP = 9
        BASS_CONFIG_GVOL_MUSIC = 6
        BASS_CONFIG_GVOL_SAMPLE = 4
        BASS_CONFIG_GVOL_STREAM = 5
        BASS_CONFIG_HANDLES = &H29
        BASS_CONFIG_MF_VIDEO = &H30
        BASS_CONFIG_MIDI_AUTOFONT = &H10402
        BASS_CONFIG_MIDI_COMPACT = &H10400
        BASS_CONFIG_MIDI_DEFFONT = &H10403
        BASS_CONFIG_MIDI_VOICES = &H10401
        BASS_CONFIG_MIXER_BUFFER = &H10601
        BASS_CONFIG_MIXER_FILTER = &H10600
        BASS_CONFIG_MIXER_POSEX = &H10602
        BASS_CONFIG_MP3_ERRORS = &H23
        BASS_CONFIG_MP4_VIDEO = &H10700
        BASS_CONFIG_MUSIC_VIRTUAL = &H16
        BASS_CONFIG_NET_AGENT = &H10
        BASS_CONFIG_NET_BUFFER = 12
        BASS_CONFIG_NET_PASSIVE = &H12
        BASS_CONFIG_NET_PLAYLIST = &H15
        BASS_CONFIG_NET_PREBUF = 15
        BASS_CONFIG_NET_PROXY = &H11
        BASS_CONFIG_NET_READTIMEOUT = &H25
        BASS_CONFIG_NET_TIMEOUT = 11
        BASS_CONFIG_OGG_PRESCAN = &H2F
        BASS_CONFIG_PAUSE_NOPLAY = 13
        BASS_CONFIG_REC_BUFFER = &H13
        BASS_CONFIG_SPLIT_BUFFER = &H10610
        BASS_CONFIG_SRC = &H2B
        BASS_CONFIG_SRC_SAMPLE = &H2C
        BASS_CONFIG_UNICODE = &H2A
        BASS_CONFIG_UPDATEPERIOD = 1
        BASS_CONFIG_UPDATETHREADS = &H18
        BASS_CONFIG_VERIFY = &H17
        BASS_CONFIG_VISTA_SPEAKERS = &H26
        BASS_CONFIG_VISTA_TRUEPOS = 30
        BASS_CONFIG_WINAMP_INPUT_TIMEOUT = &H10800
        BASS_CONFIG_WMA_ASYNC = &H1010F
        BASS_CONFIG_WMA_BASSFILE = &H10103
        BASS_CONFIG_WMA_NETSEEK = &H10104
        BASS_CONFIG_WMA_PREBUF = &H10101
        BASS_CONFIG_WMA_VIDEO = &H10105
    End Enum


#End Region


    ' Retrieve the version number of BASS that is loaded. RETURN : The BASS version (LOWORD.HIWORD)
    Declare Function BASS_GetVersion Lib "bass.dll" () As Integer


    Declare Function BASS_ErrorGetCode Lib "bass.dll" () As BassError
    '  Declare Function BASS_GetDeviceInfo Lib "bass.dll" (ByVal device As Long, ByRef info As BASS_DEVICEINFO) As IntPtr

#Region "Bass Init"
    Public Function BASS_Init(device As Integer, freq As Integer, flags As BASSInit, win As IntPtr) As Boolean
        Return BASS_Init(device, freq, flags, win, IntPtr.Zero)
    End Function

    Public Function BASS_Init(device As Integer, freq As Integer, flags As BASSInit, win As IntPtr, clsid As System.Guid) As Boolean
        If clsid.Equals(System.Guid.Empty) Then
            Return BASS_Init(device, freq, flags, win, IntPtr.Zero)
        End If
        Return BASS_InitGuid(device, freq, flags, win, clsid)
    End Function

    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Private Function BASS_Init(a0 As Integer, a1 As Integer, a2 As BASSInit, a3 As IntPtr, a4 As IntPtr) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function
    <DllImport("bass.dll", EntryPoint:="BASS_Init", CharSet:=CharSet.Auto)> _
    Private Function BASS_InitGuid(a0 As Integer, a1 As Integer, a2 As BASSInit, a3 As IntPtr, <MarshalAs(UnmanagedType.LPStruct)> A_4 As System.Guid) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <Flags> _
    Public Enum BASSInit
        BassDevice_3D = 4
        BassDevice_8Bits = 1
        BassDeviceCpspeakers = &H400
        BassDeviceDefault = 0
        BassDeviceFreq = &H4000
        BassDeviceLatency = &H100
        BassDeviceMono = 2
        BassDeviceNospeaker = &H1000
        BassDeviceSpeakers = &H800
        BassDevideDmix = &H2000
    End Enum

#End Region



    Public Enum Bass_Device
        '**********************
        '* Device setup flags *
        '**********************
        Device_Default = 0
        Device_8Bits = 1    'use 8 Bit Resolution, Else 16 Bit
        Device_Mono = 2    'use Mono, Else Stereo
        Device_3D = 4    'enable 3D Functionality
        ' If The Bass_Device_3D Flag Is Not Specified When Initilizing Bass,
        ' Then The 3D Flags (Bass_Sample_3D AndAlso Bass_Music_3D) Are Ignored When
        ' Loading/Creating A Sample/Stream/Music.
        'Device_Leavevol = 32		  'leave Volume As It Is
        'Device_Nothread = 128		  'update Buffers Manually (Using Bass_Update)
        Device_Latency = 256    'calculate Device Latency (Bass_Info Struct)
        'Device_Vol1000 = 512		  '0-1000 Volume Range (Else 0-100)
        'Device_Floatdsp = 1024		  'all Dsp Uses 32-Bit Floating-Point Data
        Device_Speakers = 2048    'force Enabling Of Speaker Assignment
    End Enum    ' Bass_Device


#Region "Volume"
    ' Set the digital output master volume.
    ' volume : Desired volume level (0-100)
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_SetVolume(volume As Single) As Boolean
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_GetVolume() As Single
    End Function
#End Region


    Declare Function BASS_PluginLoad Lib "bass.dll" (ByVal filename As String, ByVal flags As Long) As Long
    Declare Function BASS_PluginFree Lib "bass.dll" (ByVal handle As Long) As Long
    Declare Function BASS_PluginGetInfo_ Lib "bass.dll" Alias "BASS_PluginGetInfo" (ByVal handle As Long) As Long

    Declare Function BASS_Set3DFactors Lib "bass.dll" (ByVal distf As Single, ByVal rollf As Single,
                                                      ByVal doppf As Single) As Long
    Declare Function BASS_Get3DFactors Lib "bass.dll" (ByRef distf As Single, ByRef rollf As Single,
                                                      ByRef doppf As Single) As Long
    Declare Function BASS_Set3DPosition Lib "bass.dll" (ByRef pos As Object, ByRef vel As Object, ByRef front As Object,
                                                       ByRef top As Object) As Long
    Declare Function BASS_Get3DPosition Lib "bass.dll" (ByRef pos As Object, ByRef vel As Object, ByRef front As Object,
                                                       ByRef top As Object) As Long
    Declare Function BASS_Apply3D Lib "bass.dll" () As Long
    Declare Function BASS_SetEAXParameters Lib "bass.dll" (ByVal env As Long, ByVal vol As Single, ByVal decay As Single,
                                                          ByVal damp As Single) As Long
    Declare Function BASS_GetEAXParameters Lib "bass.dll" (ByRef env As Long, ByRef vol As Single, ByRef decay As Single,
                                                          ByRef damp As Single) As Long

    Declare Function BASS_MusicLoad64 Lib "bass.dll" Alias "BASS_MusicLoad" (ByVal mem As Long, ByVal file As Object,
                                                                            ByVal offset As Long,
                                                                            ByVal offsethigh As Long,
                                                                            ByVal length As Long, ByVal flags As Long,
                                                                            ByVal freq As Long) As Long
    Declare Function BASS_MusicFree Lib "bass.dll" (ByVal handle As Long) As Long

    Declare Function BASS_SampleLoad64 Lib "bass.dll" Alias "BASS_SampleLoad" (ByVal mem As Long, ByVal file As Object,
                                                                              ByVal offset As Long,
                                                                              ByVal offsethigh As Long,
                                                                              ByVal length As Long, ByVal max As Long,
                                                                              ByVal flags As Long) As Long
    Declare Function BASS_SampleCreate Lib "bass.dll" (ByVal length As Long, ByVal freq As Long, ByVal chans As Long,
                                                      ByVal max As Long, ByVal flags As Long) As Long
    Declare Function BASS_SampleFree Lib "bass.dll" (ByVal handle As Long) As Long
    Declare Function BASS_SampleSetData Lib "bass.dll" (ByVal handle As Long, ByRef buffer As Object) As Long
    Declare Function BASS_SampleGetData Lib "bass.dll" (ByVal handle As Long, ByRef buffer As Object) As Long
    Declare Function BASS_SampleGetInfo Lib "bass.dll" (ByVal handle As Long, ByRef info As BassSample) As Long
    Declare Function BASS_SampleSetInfo Lib "bass.dll" (ByVal handle As Long, ByRef info As BassSample) As Long
    Declare Function BASS_SampleGetChannel Lib "bass.dll" (ByVal handle As Long, ByVal onlynew As Long) As Long
    Declare Function BASS_SampleGetChannels Lib "bass.dll" (ByVal handle As Long, ByRef CHANNELS As Long) As Long
    Declare Function BASS_SampleStop Lib "bass.dll" (ByVal handle As Long) As Long

    Public Function BASS_StreamCreate(freq As Integer, chans As Integer, flags As BassFlag, proc As IntPtr) As Integer
        Return BASS_StreamCreate(freq, chans, flags, New IntPtr(CInt(proc)), IntPtr.Zero)
    End Function


    Public Function BASS_StreamCreateFile(memory As IntPtr, offset As Long, length As Long, flags As BassFlag) As Integer
        Return BASS_StreamCreateFileMemory(True, memory, offset, length, flags)
    End Function

    Public Function BASS_StreamCreateFile(file As String, offset As Long, length As Long, flags As BassFlag) As Integer
        flags = flags Or BassFlag.BassDefault Or BassFlag.BASS_UNICODE
        Return BASS_StreamCreateFileUnicode(False, file, offset, length, flags)
    End Function

    <DllImport("bass.dll", EntryPoint:="BASS_StreamCreateFile", CharSet:=CharSet.Auto)> _
    Private Function BASS_StreamCreateFileMemory(<MarshalAs(UnmanagedType.Bool)> A_0 As Boolean, A_1 As IntPtr, A_2 As Long, A_3 As Long, A_4 As BassFlag) As Integer
    End Function
    <DllImport("bass.dll", EntryPoint:="BASS_StreamCreateFile", CharSet:=CharSet.Auto)> _
    Private Function BASS_StreamCreateFileUnicode(<MarshalAs(UnmanagedType.Bool)> A_0 As Boolean, <[In], MarshalAs(UnmanagedType.LPWStr)> A_1 As String, A_2 As Long, A_3 As Long, A_4 As BassFlag) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamCreateFileUser(system As BASSStreamSystem, flags As BassFlag, procs As BASS_FILEPROCS, user As IntPtr) As Integer
    End Function


    Public Enum BassStreamSystem
        StreamfileNobuffer
        StreamfileBuffer
        StreamfileBufferpush
    End Enum




    ' Declare Function BASS_StreamCreateURL Lib "bass.dll" (ByVal url As String, ByVal offset As Int32, ByVal flags As Int32, ByVal proc As DOWNLOADPROC_Handler, ByVal user As Int32) As Int32
    '  Declare Function BASS_StreamCreateURL Lib "bass.dll" (ByVal url As String, _
    '                                                     ByVal offset As Long, ByVal flags As Long, ByVal proc As Long, ByVal user As IntPtr) As IntPtr

    ' Create a sample stream from an MP3/MP2/MP1/OGG orelse WAV file on the internet.
    ' url    : The URL (beginning with "http://" orelse "ftp://")
    ' offset : File offset of start streaming from
    ' flags  : Flags
    ' proc   : The callback function. Use AddressOf
    ' user   : The 'user' value passed to the callback function
    ' RETURN : The created stream's PlaybackStream (NULL=error)
    Public Function BASS_StreamCreateURL(url As String, offset As Integer, flags As BASSFlag, proc As DOWNLOADPROC, user As IntPtr) As Integer
        flags = flags Or BASSFlag.BassDefault Or BASSFlag.BASS_UNICODE
        Dim num As Integer = BASS_StreamCreateURLUnicode(url, offset, flags, proc, user)
        If num = 0 Then
            flags = flags And BASSFlag.BassAsyncfile Or BASSFlag.BASS_SPEAKER_REAR2RIGHT Or BASSFlag.BASS_SPEAKER_CENTER Or BASSFlag.BassDshowStreamLoop Or BASSFlag.BassMidiSincinter Or BASSFlag.BassAacStereo Or BASSFlag.BassMusicDecode Or BASSFlag.BassMusicNosample Or BASSFlag.BassDshowNoaudioProc Or BASSFlag.BassMusicAutofree Or BASSFlag.BASS_SAMPLE_OVER_DIST Or BASSFlag.BassMidiNocrop Or BASSFlag.BassMidiDecayseek Or BASSFlag.BassMidiNofx Or BASSFlag.BassMidiDecayend Or BASSFlag.BassAc3DynamicRange Or BASSFlag.BassAc3DownmixDolby Or BASSFlag.BassMusicFloat Or BASSFlag.BassMusicFx Or BASSFlag.BASS_SAMPLE_VAM Or BASSFlag.BassSampleMutemax Or BASSFlag.BASS_SAMPLE_SOFTWARE Or BASSFlag.BassMusic_3D Or BASSFlag.BassMusicLoop Or BASSFlag.BassFxBpmMult2 Or BASSFlag.BassFxBpmBkgrnd
            num = BASS_StreamCreateURLAscii(url, offset, flags, proc, user)
        End If
        Return num
    End Function

    <DllImport("bass.dll", EntryPoint:="BASS_StreamCreateURL", CharSet:=CharSet.Auto)> _
    Private Function BASS_StreamCreateURLAscii(<[In], MarshalAs(UnmanagedType.LPStr)> A_0 As String, A_1 As Integer, A_2 As BASSFlag, A_3 As DOWNLOADPROC, A_4 As IntPtr) As Integer
    End Function
    <DllImport("bass.dll", EntryPoint:="BASS_StreamCreateURL", CharSet:=CharSet.Auto)> _
    Private Function BASS_StreamCreateURLUnicode(<[In], MarshalAs(UnmanagedType.LPWStr)> A_0 As String, A_1 As Integer, A_2 As BASSFlag, A_3 As DOWNLOADPROC, A_4 As IntPtr) As Integer
    End Function


    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_Free() As Boolean
    End Function

    Declare Function BASS_StreamFree Lib "bass.dll" (ByVal handle As IntPtr) As Long
    Declare Function BASS_StreamGetFilePosition Lib "bass.dll" (ByVal handle As IntPtr,
                                                               ByVal mode As BASSStreamFilePosition) As Long

    Declare Function BASS_RecordGetDeviceInfo Lib "bass.dll" (ByVal device As Long, ByRef info As BASS_DEVICEINFO) _
        As Long
    Declare Function BASS_RecordInit Lib "bass.dll" (ByVal device As Long) As Long
    Declare Function BASS_RecordSetDevice Lib "bass.dll" (ByVal device As Long) As Long
    Declare Function BASS_RecordGetDevice Lib "bass.dll" () As Long
    Declare Function BASS_RecordFree Lib "bass.dll" () As Long
    Declare Function BASS_RecordGetInfo Lib "bass.dll" (ByRef info As BassRecordinfo) As Long
    Declare Function BASS_RecordGetInputName Lib "bass.dll" (ByVal inputn As Long) As Long
    Declare Function BASS_RecordSetInput Lib "bass.dll" (ByVal inputn As Long, ByVal flags As Long,
                                                        ByVal volume As Single) As Long
    Declare Function BASS_RecordGetInput Lib "bass.dll" (ByVal inputn As Long, ByRef volume As Single) As Long
    Declare Function BASS_RecordStart Lib "bass.dll" (ByVal freq As Long, ByVal chans As Long, ByVal flags As Long,
                                                     ByVal proc As Long, ByVal user As Long) As Long

    <Flags> _
    Public Enum BASSMode
        BASS_MIDI_DECAYSEEK = &H4000
        BASS_MIXER_NORAMPIN = &H800000
        BASS_MUSIC_POSRESET = &H8000
        BASS_MUSIC_POSRESETEX = &H400000
        BASS_POS_BYTES = 0
        BASS_POS_CD_TRACK = 4
        BASS_POS_DECODE = &H10000000
        BASS_POS_DECODETO = &H20000000
        BASS_POS_MIDI_TICK = 2
        BASS_POS_MUSIC_ORDERS = 1
        BASS_POS_OGG = 3
    End Enum


#Region "Channels"


    Declare Function BASS_ChannelBytes2Seconds64 Lib "bass.dll" Alias "BASS_ChannelBytes2Seconds" (ByVal handle As Long,
                                                                                                  ByVal pos As Long,
                                                                                                  ByVal poshigh As Long) _
        As Double
    Declare Function BASS_ChannelSeconds2Bytes Lib "bass.dll" (ByVal handle As Long, ByVal pos As Double) As Long
    Declare Function BASS_ChannelGetDevice Lib "bass.dll" (ByVal handle As Long) As Long
    Declare Function BASS_ChannelSetDevice Lib "bass.dll" (ByVal handle As Long, ByVal device As Long) As Long
    Declare Function BASS_ChannelIsActive Lib "bass.dll" (ByVal handle As Long) As Long
    Declare Function BASS_ChannelGetInfo Lib "bass.dll" (ByVal handle As Long, ByRef info As BASS_CHANNELINFO) As Long
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetTags(handle As Integer, tags As BassTag) As IntPtr
    End Function


    Declare Function BASS_ChannelFlags Lib "bass.dll" (ByVal handle As Long, ByVal flags As Long, ByVal mask As Long) _
        As Long
    Declare Function BASS_ChannelUpdate Lib "bass.dll" (ByVal handle As Long, ByVal length As Long) As Long
    Declare Function BASS_ChannelLock Lib "bass.dll" (ByVal handle As Long, ByVal lock_ As Long) As Long
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelPlay(handle As Integer, <MarshalAs(UnmanagedType.Bool)> restart As Boolean) As Boolean
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelStop(handle As IntPtr) As Boolean
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelPause(handle As IntPtr) As Boolean
    End Function



    Declare Function BASS_ChannelIsSliding Lib "bass.dll" (ByVal handle As Long, ByVal attrib As Long) As Long
    Declare Function BASS_ChannelSet3DAttributes Lib "bass.dll" (ByVal handle As Long, ByVal mode As Long,
                                                                ByVal min As Single, ByVal max As Single,
                                                                ByVal iangle As Long, ByVal oangle As Long,
                                                                ByVal outvol As Single) As Long
    Declare Function BASS_ChannelGet3DAttributes Lib "bass.dll" (ByVal handle As Long, ByRef mode As Long,
                                                                ByRef min As Single, ByRef max As Single,
                                                                ByRef iangle As Long, ByRef oangle As Long,
                                                                ByRef outvol As Single) As Long
    Declare Function BASS_ChannelSet3DPosition Lib "bass.dll" (ByVal handle As Long, ByRef pos As Object,
                                                              ByRef orient As Object, ByRef vel As Object) As Long
    Declare Function BASS_ChannelGet3DPosition Lib "bass.dll" (ByVal handle As Long, ByRef pos As Object,
                                                              ByRef orient As Object, ByRef vel As Object) As Long
    Declare Function BASS_ChannelGetLength Lib "bass.dll" (ByVal handle As Long, ByVal mode As Long) As Long
    Declare Function BASS_ChannelSetPosition64 Lib "bass.dll" Alias "BASS_ChannelSetPosition" (ByVal handle As Long,
                                                                                              ByVal pos As Long,
                                                                                              ByVal poshigh As Long,
                                                                                              ByVal mode As Long) _
        As Long
    Public Function BASS_ChannelGetPosition(handle As Integer) As Long
        Return BASS_ChannelGetPosition(handle, BASSMode.BASS_POS_BYTES)
    End Function

    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetPosition(handle As Integer, mode As BASSMode) As Long
    End Function



    Declare Function BASS_ChannelGetLevel Lib "bass.dll" (ByVal handle As Long) As Long
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetData(handle As Integer, buffer As IntPtr, length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetData(handle As Integer, <[In], Out> buffer As Byte(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetData(handle As Integer, <[In], Out> buffer As Short(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetData(handle As Integer, <[In], Out> buffer As Integer(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelGetData(handle As Integer, <[In], Out> buffer As Single(), length As Integer) As Integer
    End Function





    ' Setup a sync on a channel. Multiple syncs may be used per channel.
    ' PlaybackStream : Channel PlaybackStream (currently there are only HMUSIC syncs)
    ' atype  : Sync type (BASS_SYNC_xxx type & flags)
    ' param  : Sync parameters (see the BASS_SYNC_xxx type description)
    ' proc   : User defined callback function (use AddressOf SYNCPROC)
    ' user   : The 'user' value passed to the callback function
    ' Return : Sync PlaybackStream(Null = Error)
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_ChannelSetSync(handle As Integer, type As SetSyncSyncType, param As Long, proc As Syncproc, user As IntPtr) As Integer
    End Function
#End Region


    '  Function BASS_ChannelSetSync(ByVal PlaybackStream As Long, ByVal type_ As Long, ByVal param As Long, ByVal proc As Long, ByVal user As Long) As Long
    '  BASS_ChannelSetSync = BASS_ChannelSetSync(PlaybackStream, type_, param, 0, proc, user)
    '   End Function
    Public Enum SetSyncSyncType As Integer
        BASS_SYNC_MESSAGE = &H20000000
        'FLAG: sync at mixtime, else at playtime
        BASS_SYNC_MIXTIME = &H40000000
        ' FLAG: sync only once, else continuously
        BASS_SYNC_ONETIME = &H80000000

        BASS_SYNC_POS = 0
        BASS_SYNC_END = 2
        BASS_SYNC_MUSICINST = 1
        BASS_SYNC_MUSICFX = 3
        BASS_SYNC_META = 4
        BASS_SYNC_SLIDE = 5
        BASS_SYNC_STALL = 6
        BASS_SYNC_DOWNLOAD = 7
    End Enum

    Public Enum BASSStreamFilePosition
        BASS_FILEPOS_CURRENT _
        'Position that is to be decoded for playback next. This will be a bit ahead of the position actually being heard due to buffering.
        BASS_FILEPOS_END _
        'End of the file, in other words the file length. When streaming in blocks, the file length is unknown, so the download buffer length is returned instead.
        BASS_FILEPOS_START  'Start of stream data in the file.
        BASS_FILEPOS_DOWNLOAD   'Download progress of an internet file stream or "buffered" user file stream.
        BASS_FILEPOS_CONNECTED _
        'Internet file stream or "buffered" user file stream is still connected? 0 = no, 1 = yes.
        BASS_FILEPOS_BUFFER _
        'The amount of data in the buffer of an internet file stream or "buffered" user file stream. Unless streaming in blocks,this i
    End Enum

    Private Declare Sub CopyMemory Lib "kernel32" Alias "RtlMoveMemory" (ByRef Destination As Object,
                                                                        ByRef Source As Object, ByVal length As Long)


    Public Declare Function lstrlen Lib "kernel32" Alias "lstrlenA" (ByVal lpString As IntPtr) As Int32


    <DllImport("bass.dll", EntryPoint:="BASS_StreamCreate", CharSet:=CharSet.Auto)> _
    Private Function BASS_StreamCreate(freq As Int32, chans As Int32, flags As BassFlag, proc As IntPtr, user As IntPtr) As Integer
    End Function
    Public Function BASS_StreamCreatePush(freq As Integer, chans As Integer, flags As BassFlag, user As STREAMPROC) As Integer
        Dim lp As IntPtr = IntPtr.Zero
        If user IsNot Nothing Then lp = Marshal.GetFunctionPointerForDelegate(Of STREAMPROC)(user)
        Return BASS_StreamCreate(freq, chans, flags, New IntPtr(-1), lp)
    End Function

    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutData(handle As IntPtr, buffer As IntPtr, length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutData(handle As IntPtr, buffer As Byte(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutData(handle As IntPtr, buffer As Int32(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutFileData(handle As Integer, buffer As IntPtr, length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutFileData(handle As Integer, buffer As Byte(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutFileData(handle As Integer, buffer As Short(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutFileData(handle As Integer, buffer As Integer(), length As Integer) As Integer
    End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutFileData(handle As Integer, buffer As Single(), length As Integer) As Integer
    End Function

    'Public Function Bass_StreamPutData(handle As IntPtr, buffer As Byte(), len As Int32) As Int32
    '    ReDim Preserve buffer(len - 1)
    '    Dim lpBuff As IntPtr = ToPtr(buffer)
    '    Dim result As Int32 = Bass_StreamPutData(handle, lpBuff, len)
    '    Return result
    'End Function
    <DllImport("bass.dll", CharSet:=CharSet.Auto)> _
    Public Function BASS_StreamPutData(handle As IntPtr, buffer As Single(), length As Integer) As Integer
    End Function
    Public Function ToPtr(ByVal data As Object) As Int32
        Dim h As GCHandle = GCHandle.Alloc(data, GCHandleType.Pinned)
        Dim ptr As IntPtr
        Try
            ptr = h.AddrOfPinnedObject()
        Finally
            If h.IsAllocated() Then h.Free()
        End Try
        Return ptr
    End Function



    Public Function BASS_SPEAKER_N(ByVal n As Long) As Long
        BASS_SPEAKER_N = n * (2 ^ 24)
    End Function

    ' 32-bit wrappers for 64-bit BASS functions
    Function BASS_MusicLoad(ByVal mem As Long, ByVal file As Long, ByVal offset As Long, ByVal length As Long,
                            ByVal flags As Long, ByVal freq As Long) As Long
        BASS_MusicLoad = BASS_MusicLoad64(mem, file, offset, 0, length, flags Or BASS_UNICODE, freq)
    End Function

    Function BASS_SampleLoad(ByVal mem As Long, ByVal file As Long, ByVal offset As Long, ByVal length As Long,
                             ByVal max As Long, ByVal flags As Long) As Long
        BASS_SampleLoad = BASS_SampleLoad64(mem, file, offset, 0, length, max, flags Or BASS_UNICODE)
    End Function


    Function BASS_ChannelBytes2Seconds(ByVal handle As Long, ByVal pos As Long) As Double
        BASS_ChannelBytes2Seconds = BASS_ChannelBytes2Seconds64(handle, pos, 0)
    End Function

    Function BASS_ChannelSetPosition(ByVal handle As Long, ByVal pos As Long, ByVal mode As Long) As Long
        BASS_ChannelSetPosition = BASS_ChannelSetPosition64(handle, pos, 0, mode)
    End Function

    Public Function LoByte(ByVal lparam As Long) As Long
        LoByte = lparam And &HFF&
    End Function

    Public Function HiByte(ByVal lparam As Long) As Long
        HiByte = (lparam And &HFF00&) / &H100&
    End Function

    Public Function LoWord(ByVal lparam As Long) As Long
        LoWord = lparam And &HFFFF&
    End Function

    Public Function HiWord(ByVal lparam As Long) As Long
        If lparam < 0 Then
            HiWord = (lparam \ &H10000 - 1) And &HFFFF&
        Else
            HiWord = lparam \ &H10000
        End If
    End Function

    Function MakeWord(ByVal LoByte As Long, ByVal HiByte As Long) As Long
        MakeWord = (LoByte And &HFF&) Or ((HiByte And &HFF&) * &H100&)
    End Function

    Function MakeLong(ByVal LoWord As Long, ByVal HiWord As Long) As Long
        MakeLong = LoWord And &HFFFF&
        HiWord = HiWord And &HFFFF&
        If HiWord And &H8000& Then
            MakeLong = MakeLong Or (((HiWord And &H7FFF&) * &H10000) Or &H80000000)
        Else
            MakeLong = MakeLong Or (HiWord * &H10000)
        End If
    End Function


    Public Function StringToPointer(ByVal str As [String]) As IntPtr
        If str Is Nothing Then
            Return IntPtr.Zero
        Else
            Dim encoding__1 As Encoding = Encoding.UTF8
            Dim bytes As [Byte]() = encoding__1.GetBytes(str)
            Dim length As UInteger = bytes.Length + 1
            Dim pointer As IntPtr = HeapAlloc(GetProcessHeap(), 0, length)
            Marshal.Copy(bytes, 0, pointer, bytes.Length)
            Marshal.WriteByte(pointer, bytes.Length, 0)
            Return pointer
        End If
    End Function

    Public Function PointerToString(ByVal ptr As IntPtr) As [String]
        If ptr = IntPtr.Zero Then Return Nothing
        Dim encoding__1 As Encoding = Encoding.UTF8
        Dim length As Integer = GetPointerLenght(ptr)
        Dim bytes As [Byte]() = New [Byte](length - 1) {}
        Marshal.Copy(ptr, bytes, 0, length)
        Return encoding__1.GetString(bytes, 0, length)
    End Function

    Public Function GetPointerLenght(ByVal ptr As IntPtr) As Integer
        If ptr = IntPtr.Zero Then Return 0
        Return lstrlen(ptr)
    End Function

    Public Declare Function VarPtrAny Lib "vb40032.dll" Alias "VarPtr" (ByVal lpObject As Object) As Long


    <DllImport("kernel32")>
    Public Function GetProcessHeap() As IntPtr
    End Function

    <DllImport("kernel32")>
    Public Function HeapAlloc(ByVal heap As IntPtr, ByVal flags As UInt32, ByVal bytes As UInt32) As IntPtr
    End Function


#Region " BASS Callback Delegate Functions "
    Public Delegate Sub Syncproc(handle As Integer, channel As Integer, data As Integer, user As IntPtr)
    Public Delegate Sub Downloadproc(buffer As IntPtr, length As Integer, user As IntPtr)
    Public Delegate Function Streamproc(handle As IntPtr, buffer As IntPtr, length As Int32, user As IntPtr) As Int32
    Public Delegate Sub Filecloseproc(user As IntPtr)
    Public Delegate Function Filelenproc(user As IntPtr) As Long
    Public Delegate Function Filereadproc(buffer As IntPtr, length As Integer, user As IntPtr) As Integer
    Public Delegate Function FilereadprocEx(buffer As Byte(), length As Integer, user As IntPtr) As Integer
    Public Delegate Function Fileseekproc(offset As Long, user As IntPtr) As Boolean





#End Region
End Module