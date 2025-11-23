using NUnit.Framework;
using UnityEngine;

public class Plane4DTests
{
    private const float k_Epsilon = 0.0001f;

    [Test]
    public void DefaultConstructor_CreatesPlaneWithDefaultValues()
    {
        Plane4D plane = new Plane4D();

        Assert.AreEqual(new Vector4(0, 0, 0, 1), plane.normal);
        Assert.AreEqual(0f, plane.offset);
        Assert.AreEqual(Vector4.zero, plane.point);
    }

    [Test]
    public void ParameterizedConstructor_SetsValuesCorrectly()
    {
        Vector4 normal = new Vector4(1, 0, 0, 0);
        Vector4 basePoint = new Vector4(1, 2, 3, 4);
        float offset = 5f;

        Plane4D plane = new Plane4D(normal, basePoint, offset);

        Assert.AreEqual(normal.normalized, plane.normal);
        Assert.AreEqual(offset, plane.offset);
    }

    [Test]
    public void Constructor_NormalizesNormal()
    {
        Vector4 unnormalizedNormal = new Vector4(2, 0, 0, 0);
        Vector4 basePoint = Vector4.zero;

        Plane4D plane = new Plane4D(unnormalizedNormal, basePoint);

        Assert.AreEqual(1f, plane.normal.magnitude, k_Epsilon);
        Assert.AreEqual(new Vector4(1, 0, 0, 0), plane.normal);
    }

    [Test]
    public void NormalSetter_NormalizesValue()
    {
        Plane4D plane = new Plane4D();
        Vector4 unnormalizedNormal = new Vector4(0, 3, 0, 0);

        plane.normal = unnormalizedNormal;

        Assert.AreEqual(1f, plane.normal.magnitude, k_Epsilon);
        Assert.AreEqual(new Vector4(0, 1, 0, 0), plane.normal);
    }

    [Test]
    public void Point_ReturnsBasePointPlusNormalTimesOffset()
    {
        Vector4 normal = new Vector4(1, 0, 0, 0);
        Vector4 basePoint = new Vector4(0, 5, 0, 0);
        float offset = 3f;

        Plane4D plane = new Plane4D(normal, basePoint, offset);

        Vector4 expectedPoint = basePoint + normal.normalized * offset;
        Assert.AreEqual(expectedPoint.x, plane.point.x, k_Epsilon);
        Assert.AreEqual(expectedPoint.y, plane.point.y, k_Epsilon);
        Assert.AreEqual(expectedPoint.z, plane.point.z, k_Epsilon);
        Assert.AreEqual(expectedPoint.w, plane.point.w, k_Epsilon);
    }

    [Test]
    public void Point_WithZeroOffset_ReturnsBasePoint()
    {
        Vector4 normal = new Vector4(0, 0, 1, 0);
        Vector4 basePoint = new Vector4(1, 2, 3, 4);

        Plane4D plane = new Plane4D(normal, basePoint, 0f);

        Assert.AreEqual(basePoint.x, plane.point.x, k_Epsilon);
        Assert.AreEqual(basePoint.y, plane.point.y, k_Epsilon);
        Assert.AreEqual(basePoint.z, plane.point.z, k_Epsilon);
        Assert.AreEqual(basePoint.w, plane.point.w, k_Epsilon);
    }

    [Test]
    public void Point_WithNegativeOffset_MovesInOppositeDirection()
    {
        Vector4 normal = new Vector4(0, 0, 0, 1);
        Vector4 basePoint = new Vector4(0, 0, 0, 10);
        float offset = -5f;

        Plane4D plane = new Plane4D(normal, basePoint, offset);

        Vector4 expectedPoint = new Vector4(0, 0, 0, 5);
        Assert.AreEqual(expectedPoint.x, plane.point.x, k_Epsilon);
        Assert.AreEqual(expectedPoint.y, plane.point.y, k_Epsilon);
        Assert.AreEqual(expectedPoint.z, plane.point.z, k_Epsilon);
        Assert.AreEqual(expectedPoint.w, plane.point.w, k_Epsilon);
    }

    [Test]
    public void OffsetSetter_ChangesPointAccordingly()
    {
        Vector4 normal = new Vector4(1, 0, 0, 0);
        Vector4 basePoint = Vector4.zero;
        Plane4D plane = new Plane4D(normal, basePoint, 0f);

        plane.offset = 10f;

        Assert.AreEqual(10f, plane.point.x, k_Epsilon);
        Assert.AreEqual(0f, plane.point.y, k_Epsilon);
        Assert.AreEqual(0f, plane.point.z, k_Epsilon);
        Assert.AreEqual(0f, plane.point.w, k_Epsilon);
    }

    [Test]
    public void ChangingNormal_UpdatesPointCalculation()
    {
        Vector4 basePoint = Vector4.zero;
        float offset = 5f;
        Plane4D plane = new Plane4D(new Vector4(1, 0, 0, 0), basePoint, offset);

        Assert.AreEqual(5f, plane.point.x, k_Epsilon);
        Assert.AreEqual(0f, plane.point.y, k_Epsilon);

        plane.normal = new Vector4(0, 1, 0, 0);

        Assert.AreEqual(0f, plane.point.x, k_Epsilon);
        Assert.AreEqual(5f, plane.point.y, k_Epsilon);
    }

    [Test]
    public void DiagonalNormal_PointCalculatesCorrectly()
    {
        Vector4 normal = new Vector4(1, 1, 1, 1);
        Vector4 basePoint = Vector4.zero;
        float offset = 2f;

        Plane4D plane = new Plane4D(normal, basePoint, offset);

        float expectedComponent = offset / 2f;
        Assert.AreEqual(expectedComponent, plane.point.x, k_Epsilon);
        Assert.AreEqual(expectedComponent, plane.point.y, k_Epsilon);
        Assert.AreEqual(expectedComponent, plane.point.z, k_Epsilon);
        Assert.AreEqual(expectedComponent, plane.point.w, k_Epsilon);
    }

    [Test]
    public void MultipleOffsetChanges_PointUpdatesEachTime()
    {
        Vector4 normal = new Vector4(0, 0, 1, 0);
        Vector4 basePoint = new Vector4(1, 1, 1, 1);
        Plane4D plane = new Plane4D(normal, basePoint, 0f);

        plane.offset = 5f;
        Assert.AreEqual(6f, plane.point.z, k_Epsilon);

        plane.offset = -3f;
        Assert.AreEqual(-2f, plane.point.z, k_Epsilon);

        plane.offset = 0f;
        Assert.AreEqual(1f, plane.point.z, k_Epsilon);
    }
}
