// scratch_cpp.cpp : This file contains the 'main' function. Program execution begins and ends there.
//
#include <Windows.h>
#include <mmsystem.h>
#include <iostream>

/* The array of MIDIEVENTs to be output. We only have 2 */
unsigned long myNotes[] = { 0, 0, 0x007F3C90, /* A note-on */
192, 0, 0x00003C90 }; /* A note-off. It's the last event in the array */

HANDLE    event;

void CALLBACK midiCallback(HMIDIOUT handle, UINT uMsg, DWORD dwInstance, DWORD dwParam1, DWORD dwParam2)
{
    LPMIDIHDR   lpMIDIHeader;
    MIDIEVENT* lpMIDIEvent;

    /* Determine why Windows called me */
    switch (uMsg)
    {
        /* Got some event with its MEVT_F_CALLBACK flag set */
    case MOM_POSITIONCB:

        /* Assign address of MIDIHDR to a LPMIDIHDR variable. Makes it easier to access the
           field that contains the pointer to our block of MIDI events */
        lpMIDIHeader = (LPMIDIHDR)dwParam1;

        /* Get address of the MIDI event that caused this call */
        lpMIDIEvent = (MIDIEVENT*)&(lpMIDIHeader->lpData[lpMIDIHeader->dwOffset]);

        /* Normally, if you had several different types of events with the
           MEVT_F_CALLBACK flag set, you'd likely now do a switch on the highest
           byte of the dwEvent field, assuming that you need to do different
           things for different types of events.
        */

        break;

        /* The last event in the MIDIHDR has played */
    case MOM_DONE:

        /* Wake up main() */
        SetEvent(event);

        break;


        /* Process these messages if you desire */
    case MOM_OPEN:
    case MOM_CLOSE:

        break;
    }
}
int main(int argc, char** argv)
{
    HMIDISTRM       outHandle;
    MIDIHDR         midiHdr;
    MIDIPROPTIMEDIV prop;
    UINT  err;
    /* Allocate an Event signal */
    if ((event = CreateEvent(0, FALSE, FALSE, 0)))
    {
        /* Open default MIDI Out stream device. Tell it to notify via CALLBACK_EVENT and use my created Event */
        err = 0;
        if (!(err = midiStreamOpen(&outHandle, &err, 1, (DWORD)midiCallback, 0, CALLBACK_FUNCTION)))
        {
            /* Set the timebase. Here I use 96 PPQN */
            prop.cbStruct = sizeof(MIDIPROPTIMEDIV);
            prop.dwTimeDiv = 96;
            midiStreamProperty(outHandle, (LPBYTE)&prop, MIDIPROP_SET | MIDIPROP_TIMEDIV);

            /* If you wanted something other than 120 BPM, here you should also set the tempo */

            /* Store pointer to our stream (ie, array) of messages in MIDIHDR */
            midiHdr.lpData = (LPSTR)&myNotes[0];

            /* Store its size in the MIDIHDR */
            midiHdr.dwBufferLength = midiHdr.dwBytesRecorded = sizeof(myNotes);

            /* Flags must be set to 0 */
            midiHdr.dwFlags = 0;
            size_t hdrSiz = sizeof(MIDIHDR);
            /* Prepare the buffer and MIDIHDR */
            std::cout << hdrSiz << "\r\n";
            err = midiOutPrepareHeader((HMIDIOUT)outHandle, &midiHdr, hdrSiz);
            if (!err)
            {
                /* Queue the Stream of messages. Output doesn't actually start
                   until we later call midiStreamRestart().
                 */
                err = midiStreamOut(outHandle, &midiHdr, hdrSiz);
                if (!err)
                {
                    /* Start outputting the Stream of messages. This will return immediately
                       as the stream device will time out and output the messages on its own in
                       the background.
                     */
                    err = midiStreamRestart(outHandle);
                    if (!err)

                        /* Wait for playback to stop. Windows calls my callback, which will set this signal */
                       WaitForSingleObject(event, INFINITE);
                }

                /* Unprepare the buffer and MIDIHDR */
                midiOutUnprepareHeader((HMIDIOUT)outHandle, &midiHdr, sizeof(MIDIHDR));
            }

            /* Close the MIDI Stream */
            midiStreamClose(outHandle);
        }

        /* Free the Event */
        CloseHandle(event);
    }

    return(0);
}