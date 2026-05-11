using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cCamera : Item
{
    const int 
        pSLST_EID   = 0x0,
        pGARBAGE    = 0x4,
        pNEIG_COUNT = 0x8,
        pENTRY_POINT= 0x1C,
        pEXIT_POINT = 0x1D,
        pPOINT_COUNT= 0x1E,
        pMODE       = 0x20,
        pAVG_DIST   = 0x22,
        pZOOM       = 0x24,
        pUNKNOWN_1  = 0x26,
        pUNKNOWN_2  = 0x28,
        pUNKNOWN_3  = 0x2A,
        pX_DIR      = 0x2C,
        pY_DIR      = 0x2E,
        pZ_DIR      = 0x30,
        pPOINT_LIST = 0x32;
        
    public List<cCameraPos> camTrack;

    private byte[] rawCamera;

    public int   SLSTeid     => ConvertBits.FromInt32(rawCamera, pSLST_EID   );
    public int   garbage     => ConvertBits.FromInt16(rawCamera, pGARBAGE    );
    public int   neig_count  => ConvertBits.FromInt16(rawCamera, pNEIG_COUNT );
    public byte  entry_point => rawCamera[pENTRY_POINT];
    public byte  exit_point  => rawCamera[pEXIT_POINT ];
    public short point_count => ConvertBits.FromInt16(rawCamera, pPOINT_COUNT);
    public short mode        => ConvertBits.FromInt16(rawCamera, pMODE       );
    public short avg_dist    => ConvertBits.FromInt16(rawCamera, pAVG_DIST   );
    public short zoom        => ConvertBits.FromInt16(rawCamera, pZOOM       );
    public short unknown_1   => ConvertBits.FromInt16(rawCamera, pUNKNOWN_1  );
    public short unknown_2   => ConvertBits.FromInt16(rawCamera, pUNKNOWN_2  );
    public short unknown_3   => ConvertBits.FromInt16(rawCamera, pUNKNOWN_3  );
    public short X_dir       => ConvertBits.FromInt16(rawCamera, pX_DIR      );
    public short Y_dir       => ConvertBits.FromInt16(rawCamera, pY_DIR      );
    public short Z_dir       => ConvertBits.FromInt16(rawCamera, pZ_DIR      );
    public short point_list  => ConvertBits.FromInt16(rawCamera, pPOINT_LIST );

    public cCamera(byte[] _data) : base(_data)
    {
        rawCamera = _data;
        camTrack = new();

        for(int trackPos = 0; trackPos < point_count; trackPos++)
        {
            cCameraPos point = new cCameraPos();
            point.x    = ConvertBits.FromInt16(rawCamera, pPOINT_LIST + 0x0   + 12 * trackPos);
            point.y    = ConvertBits.FromInt16(rawCamera, pPOINT_LIST + 0x2   + 12 * trackPos);
            point.z    = ConvertBits.FromInt16(rawCamera, pPOINT_LIST + 0x4   + 12 * trackPos);
            point.xrot = ConvertBits.FromInt16(rawCamera, pPOINT_LIST + 0x6   + 12 * trackPos);
            point.yrot = ConvertBits.FromInt16(rawCamera, pPOINT_LIST + 0x8   + 12 * trackPos);
            point.zrot = ConvertBits.FromInt16(rawCamera, pPOINT_LIST + 0xA   + 12 * trackPos);

        }
    }



}

public struct cCameraPos
{
    public short x, y, z, xrot, yrot, zrot;
}
