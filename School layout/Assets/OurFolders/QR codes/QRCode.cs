using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCode
{
    private int roomId;
    private BitArray qrAsBits;

    // Constructor with default data that each QR code shall have
    public QRCode (int roomId, BitArray qrAsBits) {
        this.roomId = roomId;
        this.qrAsBits = qrAsBits;
    }
}
