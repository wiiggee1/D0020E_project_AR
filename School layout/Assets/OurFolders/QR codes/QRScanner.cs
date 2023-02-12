using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QRScanner : MonoBehaviour
{

    // Scanner variables
    WebCamTexture webcamTexture;
    string QrCode = string.Empty;

    // Create mini database of QR codes
    QRCode[] qrDB = new QRCode[5] { new QRCode(1, "one", 1.5f, 2.5f, 1.5f),
                                    new QRCode(2, "two", 2.5f, 3.5f, 1.5f),
                                    new QRCode(3, "three", 2.5f, 3.5f, 1.5f),
                                    new QRCode(4, "four", 2.5f, 3.5f, 1.5f),
                                    new QRCode(5, "five", 2.5f, 3.5f, 1.5f) };

    // Get current QR code
    QRCode currentQR;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // check input from device's camera and if found qr code, react

        if (Main.checkForQR == true)
        {
            this.ScanInit();
        }
    }

    void ScanInit()
    {
        var renderer = GetComponent<RawImage>();
        webcamTexture = new WebCamTexture(512, 512);
        renderer.texture = webcamTexture;
        StartCoroutine(ScanQR());
    }

    // Taken from https://github.com/nickdu088/Unity-QR-Scanner/blob/master/Assets/Scripts/QRScanner.cs and modified a bit
    IEnumerator ScanQR()
    {
        IBarcodeReader barCodeReader = new BarcodeReader();
        webcamTexture.Play();
        var snap = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.ARGB32, false);
        while (string.IsNullOrEmpty(QrCode))
        {
            try
            {
                snap.SetPixels32(webcamTexture.GetPixels32());
                var Result = barCodeReader.Decode(snap.GetRawTextureData(), webcamTexture.width, webcamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);
                if (Result != null)
                {
                    QrCode = Result.Text;
                    if (!string.IsNullOrEmpty(QrCode))
                    {
                        foreach (QRCode qrElem in qrDB)
                        {
                            if (qrElem.getDecodedText().Equals(QrCode))
                            {
                                this.Anchorize();
                                goto OUTOFWHILE;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Debug.LogWarning(ex.Message); }
            yield return null;
        }
        OUTOFWHILE:
        webcamTexture.Stop();
    }

    void Anchorize()
    {

    }
}
