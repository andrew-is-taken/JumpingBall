using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MovementTest
{
    [Test]
    public void RecalculateAdditionalDirectionOnRotation_Test()
    {
        MovementController controller = new MovementController();

        controller.movingHorizontally = true;

        controller.additionalDirection = new Vector3(1, 0, 0);
        controller.RecalculateAdditionalDirectionOnRotation(true);
        Assert.AreEqual(new Vector3(0, 1, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(-1, 0, 0);
        controller.RecalculateAdditionalDirectionOnRotation(true);
        Assert.AreEqual(new Vector3(0, -1, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(1, 0, 0);
        controller.RecalculateAdditionalDirectionOnRotation(false);
        Assert.AreEqual(new Vector3(0, -1, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(-1, 0, 0);
        controller.RecalculateAdditionalDirectionOnRotation(false);
        Assert.AreEqual(new Vector3(0, 1, 0), controller.additionalDirection);

        controller.movingHorizontally = false;

        controller.additionalDirection = new Vector3(0, 1, 0);
        controller.RecalculateAdditionalDirectionOnRotation(true);
        Assert.AreEqual(new Vector3(-1, 0, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(0, -1, 0);
        controller.RecalculateAdditionalDirectionOnRotation(true);
        Assert.AreEqual(new Vector3(1, 0, 0), controller.additionalDirection);
        
        controller.additionalDirection = new Vector3(0, 1, 0);
        controller.RecalculateAdditionalDirectionOnRotation(false);
        Assert.AreEqual(new Vector3(1, 0, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(0, -1, 0);
        controller.RecalculateAdditionalDirectionOnRotation(false);
        Assert.AreEqual(new Vector3(-1, 0, 0), controller.additionalDirection);
    }

    [Test]
    public void RecalculateAdditionalDirectionOnJump_Test()
    {
        MovementController controller = new MovementController();

        controller.movingHorizontally = true;

        controller.additionalDirection = new Vector3(0, 1, 0);
        controller.RecalculateAdditionalDirectionOnJump();
        Assert.AreEqual(new Vector3(0, -1, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(0, -1, 0);
        controller.RecalculateAdditionalDirectionOnJump();
        Assert.AreEqual(new Vector3(0, 1, 0), controller.additionalDirection);

        controller.movingHorizontally = false;

        controller.additionalDirection = new Vector3(1, 0, 0);
        controller.RecalculateAdditionalDirectionOnJump();
        Assert.AreEqual(new Vector3(-1, 0, 0), controller.additionalDirection);

        controller.additionalDirection = new Vector3(-1, 0, 0);
        controller.RecalculateAdditionalDirectionOnJump();
        Assert.AreEqual(new Vector3(1, 0, 0), controller.additionalDirection);
    }
}
