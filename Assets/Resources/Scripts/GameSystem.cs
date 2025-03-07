using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class holds important constants concerning all objects of the game.
public class GameSystem : MonoBehaviour
{
    // Divide all serialized speeds by this to get appropriate frame speed for in-game physics
    public const int SPEED_DIVISOR = 50;

    // Boundaries of action game screen
    public const float X_ACTION_BOUNDARY = 1.06f;
    public const float Y_ACTION_BOUNDARY = 1.28f;

    // Divide pixel distances by this to reflect in game space
    public const float PIXELS_PER_UNIT = 100;

    // Return a movement of the specified distance and angle
    public static Vector3 MoveAtAngle(float angle, float distance) {
        // Adjust because unity rotation is counterclockwise
        angle = 360 - angle;
        float xChange = (float) (distance * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
        float yChange = (float) (distance * Math.Cos(angle * (Math.PI / 180)));

        return new Vector3(xChange, yChange, 0);
    }

    // Returns Z angle for current to face target
    public static float FacePoint(Vector3 currentPosition, Vector3 target) {
        float angle;

        // Arctan formula
        angle = (float) Math.Atan((target.y - currentPosition.y) / (target.x - currentPosition.x));
        // Convert to degrees
        angle *= (float) (180 / Math.PI);
        // Add 90 for upward alignment
        angle += 90;
        // Adjust if target is on right side
        if (target.x >= currentPosition.x) {
            angle += 180;
        }

        return angle;
    }

    // Calculate distance between two points
    public static float PointDistance(Vector3 point1, Vector3 point2) {
        return (float) Math.Sqrt(Math.Pow(point2.x - point1.x, 2) + Math.Pow(point2.y - point1.y, 2));
    }

    // Returns movement a step from current towards the destination
    public static Vector3 MoveTowardsPoint(Vector3 currentPosition, Vector3 destination, float speed) {
        Vector3 result;

        // Find distance and angle between points
        float angle = FacePoint(currentPosition, destination);
        //Debug.Log(destination.x + ", " + destination.y);
        float distance = PointDistance(currentPosition, destination);

        // Move at speed towards target
        if (speed < distance) {
            result = MoveAtAngle(angle, speed);
        } else {
            result = MoveAtAngle(angle, distance);
        }

        return result;
    }

    // Flickers sprite alpha for length of time
    public static IEnumerator FlickerSprite(SpriteRenderer s, float seconds) {
        float total = 0;
        float flashTime = 0.05f; // 20th of a second
        Color temp = s.color;

        // Loop for iframe time
        while (total < seconds && s != null) {
            // Flash alpha color
            if (temp.a == 1)
                temp.a = 0f;
            else 
                temp.a = 1f;

            s.color = temp;

            // Wait for a 20th of a second
            yield return new WaitForSeconds(flashTime);

            // Add time to total
            total += flashTime;
        }

        if (s == null)
            yield break;

        // If alpha is zero after loop, set to 1
        if (temp.a == 0) {
            temp.a = 1;
            s.color = temp;
        }
    }

    // Briefly flash sprite a certain material
    public static IEnumerator FlashSprite(SpriteRenderer s, Material m) {
        Material defaultMaterial = s.material;

        // Flash for one 20th of a second
        s.material = m;
        yield return new WaitForSeconds(0.05f);
        s.material = defaultMaterial;
    }
}
