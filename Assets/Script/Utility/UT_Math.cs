using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ShapeType
{
	ST_NONE	  = 0, //无
	ST_CIRCLE = 1, //圆形
	ST_RECT	  = 2, //长方形
	ST_FAN	  = 3, //扇形
}

[System.Serializable]
public class UT_ShapeBase
{
	public ShapeType _eShapeType = ShapeType.ST_NONE;
}

[System.Serializable]
public class UT_Shape_Circle : UT_ShapeBase
{
	public UT_Shape_Circle()
	{
		_eShapeType = ShapeType.ST_CIRCLE;
	}

	public void Clone(UT_Shape_Circle kShape)
	{
		kShape._eShapeType = _eShapeType;
		kShape.kCenter = kCenter;
		kShape.fRadius = fRadius;
	}

	public Vector3 kCenter;
	public float   fRadius;
}

[System.Serializable]
public class UT_Shape_Fan : UT_ShapeBase
{
	public UT_Shape_Fan()
	{
		_eShapeType = ShapeType.ST_FAN;
	}

	public void Clone(UT_Shape_Fan kShape)
	{
		kShape._eShapeType 	= _eShapeType;
		kShape.kCenter  	= kCenter;
		kShape.kAxis 		= kAxis;
		kShape.fRadius  	= fRadius;
		kShape.fAngle 		= fAngle;
		kShape.fRotWithY 	= fRotWithY;
	}

	public Vector3 kCenter;
	public Vector3 kAxis;
	public float   fRadius;
	public float   fAngle;
	public float   fRotWithY;
}

[System.Serializable]
public class UT_Shape_Rect : UT_ShapeBase
{
	public UT_Shape_Rect()
	{
		_eShapeType = ShapeType.ST_RECT;
	}
	
	public void Clone(UT_Shape_Rect kShape)
	{
		kShape._eShapeType 	= _eShapeType;
		kShape.kBase  		= kBase;
		kShape.kAxis 		= kAxis;
		kShape.fHalfWidth 	= fHalfWidth;
		kShape.fHalfLength 	= fHalfLength;
		kShape.fHalfHeight	= fHalfHeight;
		kShape.fRotWithY 	= fRotWithY;
	}
	
	public Vector3 kBase;
	public Vector3 kAxis;
	public float   fHalfWidth;
	public float   fHalfLength;
	public float   fHalfHeight;
	public float   fRotWithY;
}


public class UT_Math
{
	public static float ComputeLineLengthXY(ref Vector3 kPoint1,ref Vector3 kPoint2)
	{
		float fDeltaX = kPoint1.x - kPoint2.x;
		float fDeltaZ = kPoint1.z - kPoint2.z;
		
		return Mathf.Sqrt (fDeltaX * fDeltaX + fDeltaZ * fDeltaZ);
	}

    public static float ComputeLineLengthXY(Vector3 kPoint1, Vector3 kPoint2)
    {
        float fDeltaX = kPoint1.x - kPoint2.x;
        float fDeltaZ = kPoint1.z - kPoint2.z;

        return Mathf.Sqrt(fDeltaX * fDeltaX + fDeltaZ * fDeltaZ);
    }

    public static float ComputeLineLengthXYSqr(ref Vector3 kPoint1,ref Vector3 kPoint2)
	{
		float fDeltaX = kPoint1.x - kPoint2.x;
		float fDeltaZ = kPoint1.z - kPoint2.z;
		
		return (fDeltaX * fDeltaX + fDeltaZ * fDeltaZ);
	}

    public static bool IsPostionEqual(Vector3 kLeft, Vector3 kRight)
    {
        Vector3 kDiff = kLeft - kRight;
        if (Mathf.Abs(kDiff.x) < 1e-5 &&
            Mathf.Abs(kDiff.y) < 1e-5 &&
            Mathf.Abs(kDiff.z) < 1e-5)
        {
            return true;
        }
        return false;
    }

	public static bool TestShapeCollision(UT_Shape_Circle kCircle1,UT_Shape_Circle kCircle2)
	{
		float fDistanceSqr = ComputeLineLengthXYSqr (ref kCircle1.kCenter, ref kCircle2.kCenter);
		float fRadiusSqr = kCircle1.fRadius + kCircle2.fRadius;
		fRadiusSqr = fRadiusSqr * fRadiusSqr;
		
		if(fDistanceSqr >= fRadiusSqr)
			return false;
		else
			return true;
	}


	public static bool TestShapeCollision(UT_Shape_Fan kFan,UT_Shape_Circle kCircle)
	{
		//先测试两个圆是否相交
		//float fDistance = Vector3.Distance(fanCenterPoint,circleCenter);
		float fDistanceSqr = ComputeLineLengthXYSqr (ref kFan.kCenter, ref kCircle.kCenter);
		float fRadiusSqr = kFan.fRadius + kCircle.fRadius;
		fRadiusSqr = fRadiusSqr * fRadiusSqr;
		if(fDistanceSqr >= fRadiusSqr)
			return false;
		
		//测试圆心是否在扇形内
		Vector3 kCenterToCenter;
		kCenterToCenter.x = kCircle.kCenter.x - kFan.kCenter.x;
		kCenterToCenter.y = 0;
		kCenterToCenter.z = kCircle.kCenter.z - kFan.kCenter.z;
		kCenterToCenter.Normalize();
		
		float fDot = Vector3.Dot(kCenterToCenter,kFan.kAxis);
		fDot = Mathf.Clamp (fDot, -0.99f, 0.99f);
		float fAngle = Mathf.Acos(fDot);
		if(Mathf.Abs(fAngle) * Mathf.Rad2Deg <= kFan.fAngle * 0.5f)
			return true;

        return false;
	}

	public static bool TestShapeCollision(UT_Shape_Rect kRect,UT_Shape_Circle kCircle)
	{
		Vector3 kRectAxisZ = new Vector3();
		kRectAxisZ.x = kRect.kAxis.z;
		kRectAxisZ.z = -kRect.kAxis.x;
		
		Vector3 kRelativePoint;
		kRelativePoint.x = kCircle.kCenter.x - kRect.kBase.x;
		kRelativePoint.z = kCircle.kCenter.z - kRect.kBase.z;
		
		float fProjectX = kRelativePoint.x * kRect.kAxis.x + kRelativePoint.z * kRect.kAxis.z;
		if(fProjectX < -kCircle.fRadius || fProjectX >= kRect.fHalfHeight*2 + kCircle.fRadius)
			return false;
		
		float fProjectZ = kRelativePoint.x * kRectAxisZ.x + kRelativePoint.z * kRectAxisZ.z;
		if(Mathf.Abs(fProjectZ) >= kRect.fHalfWidth + kCircle.fRadius)
			return false;
		
		return true;
	}

	//------------------------------------------------------------------------
	public static bool GetDigitalOnBinaryPos(uint uiValue,int iPos )
	{
		if( iPos<0 || iPos>=32 ) // out of range
		{
			return false; 
		}
		
		uint lMask = (uint)1 << iPos;
		
		if ((lMask & uiValue) > 0)
			return true;
		else
			return false;
	}

	//------------------------------------------------------------------------
	public static bool SetDigitalOnBinaryPos(uint uiValue,int iPos,bool bValue )
	{
		if( iPos<0 || iPos>=32 ) // out of range
		{
			return false; 
		}
		
		uint lMask = (uint)1 << iPos;
		
		if (bValue)
		{
			uiValue |= lMask;
		}
		else
			uiValue &= (~lMask);
		
		return true;
	}

	//------------------------------------------------------------------------
	public static int ParseInt(string kNumberStr)
	{
		int ret = 0;
		try 
		{
			ret = int.Parse (kNumberStr);
		} 
		catch
		{
			ret = 0;
		}
		return ret;
	}

	//------------------------------------------------------------------------
	public static uint ParseUInt(string kNumberStr)
	{
		uint ret = 0;
		try 
		{
			ret = uint.Parse (kNumberStr);
		} 
		catch 
		{
			ret = 0;
		}
		return ret;
	}

	//------------------------------------------------------------------------
	public static float ParseFloat(string kNumberStr)
	{
		float fRes = 0.00f;
		try 
		{
			fRes = float.Parse (kNumberStr);
		} 
		catch
		{
			fRes = 0.00f;
		}
		return fRes;
	}

    //------------------------------------------------------------------------
    public static ulong ParseULong(string kNumberStr)
    {
        ulong ret = 0;
        try
        {
            ret = ulong.Parse(kNumberStr);
        }
        catch
        {
            ret = 0;
        }
        return ret;
    }

    //------------------------------------------------------------------------
    public static long ParseLong(string kNumberStr)
    {
        long ret = 0;
        try
        {
            ret = long.Parse(kNumberStr);
        }
        catch 
        {
            ret = 0;
        }
        return ret;
    }
    //------------------------------------------------------------------------
    public static double ParseDouble(string kNumberStr)
    {
        double ret = 0;
        try
        {
            ret = double.Parse(kNumberStr);
        }
        catch
        {
            ret = 0;
        }
        return ret;
    }
    //------------------------------------------------------------------------
    public static bool IsEqual(float fA, float fB)
    {
        if (Mathf.Abs(fA - fB) <= Mathf.Epsilon)
            return true;

        return false;
    }
    //------------------------------------------------------------------------
    public static bool InRange(int iValue,int iMin,int iMax,bool bCloseLeft = true,bool bCloseRight = true)
	{
        if (iValue > iMin && iValue < iMax)
            return true;

        if (bCloseLeft && iValue == iMin)
            return true;

        if (bCloseRight && iValue == iMax)
            return true;

		return false;
	}
    //------------------------------------------------------------------------
    public static bool InRange(float fValue, float fMin, float fMax, bool bCloseLeft = true, bool bCloseRight = true)
    {
        if (fValue > fMin && fValue < fMax)
            return true;

        if (bCloseLeft && IsEqual(fValue,fMin))
            return true;

        if (bCloseRight && IsEqual(fValue, fMax))
            return true;

        return false;
    }
    //------------------------------------------------------------------------
    // Project vector on plane
    public static Vector3 ProjectVectorOnPlane(Vector3 kPlaneNormal, Vector3 kVector)
    {
        return kVector - (Vector3.Dot(kVector, kPlaneNormal) * kPlaneNormal);
    }
    //------------------------------------------------------------------------
    // Get signed vector angle
    public static float SignedVectorAngle(Vector3 kReferenceVector, Vector3 kOtherVector, Vector3 kNormal)
    {
        Vector3 kPerpVector;
        float fAngle;

        kPerpVector = Vector3.Cross(kNormal, kReferenceVector);
        fAngle = Vector3.Angle(kReferenceVector, kOtherVector);
        fAngle *= Mathf.Sign(Vector3.Dot(kPerpVector, kOtherVector));
        return fAngle;
    }
    //------------------------------------------------------------------------
    public static float GetVectorLengthOnXZ(Vector3 kStart, Vector3 kEnd)
    {
        Vector3 kVector = kEnd - kStart;
        kVector.y = 0;
        return kVector.magnitude;
    }

    ///List随机排序
    public static List<Vector3> RandomList(List<Vector3> kSource)
    {
        return kSource.OrderBy(x => System.Guid.NewGuid()).ToList<Vector3>();
    }
}
