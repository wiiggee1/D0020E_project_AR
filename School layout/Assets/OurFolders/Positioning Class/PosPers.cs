using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosPers
{
    private float x;
    private float y;
    private float z;
    private float upDown;
    private float leftRight;

    public PosPers( float x, float y, float z, float upDown, float leftRight ) 
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.upDown = upDown;
        this.leftRight = leftRight;
    }

    
    public void setPosPers( float x, float y, float z, float upDown, float leftRight ) 
    {
        this.x = x; 
        this.y = y; 
        this.z = z; 
        this.upDown = upDown;
        this.leftRight = leftRight;
    }

    public ( float x, float y, float z, float upDown, float leftRight ) getPos() 
    { 
        return ( x, y, z, upDown, leftRight ); 
    }
    /*
    public void setPos( float x, float y, float z ) 
    {
        this.x = x; 
        this.y = y; 
        this.z = z; 
    } 

    public ( float x, float y, float z ) getPos() 
    { 
        return ( x, y, z ); 
    }

    public void setPers( float upDown, float leftRight ) 
    {
        this.upDown = upDown;
        this.leftRight = leftRight;
    }
    
    public ( float upDown, float leftRight ) getPers() 
    { 
        return ( upDown, leftRight ); 
    }
    */
}
