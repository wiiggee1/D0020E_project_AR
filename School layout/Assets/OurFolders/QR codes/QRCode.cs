using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Does not need to be inheriting from MonoBehaviour
public class QRCode
{
    // Local variables to store default data for each QR code
    private int roomId;
    private string decodedText;
    private float pic_x;
    private float pic_y;
    private float pic_z;
    // Size does not matter as we assume to take bottom left corner
    // Another assumption: QR code vertically. Straight. Wall?

    // Constructor with default data that each QR code shall have
    public QRCode (int roomId, string decodedText, float pic_x, float pic_y, float pic_z) {
        this.roomId = roomId;
        this.decodedText = decodedText;
        this.pic_x = pic_x;
        this.pic_y = pic_y;
        this.pic_z = pic_z;
    }

    public string getDecodedText()
    {
        return decodedText;
    }
    public float getPic_x()
    {
        return pic_x;
    }
    public float getPic_y()
    {
        return pic_y;
    }
    public float getPic_z()
    {
        return pic_z;
    }
}
