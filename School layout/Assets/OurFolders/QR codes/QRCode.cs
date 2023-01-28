using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not need to be inheriting from MonoBehaviour
public class QRCode
{
    // Local variables to store default data for each QR code
    private int roomId;
    // TODO: add location variables

    // Constructor with default data that each QR code shall have
    public QRCode (int roomId, BitArray qrAsBits) {
        this.roomId = roomId;
    }
}
