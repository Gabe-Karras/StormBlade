using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class holds important constants concerning all objects of the game.
public class GameSystem : MonoBehaviour
{
    // Frames per second!!!
    public static float FRAME_RATE = 60;

    public static bool gameInitialized = false;

    // Divide all serialized speeds by this to get appropriate frame speed for in-game physics
    public static float SPEED_DIVISOR = 50;
    public static float ROTATION_DIVISOR = 1;
    public const float FRAME_SPEED_RATIO = 1.2f;
    public static float FRAME_TIME_RATIO = 1;

    // Boundaries of action game screen
    public const float X_ACTION_BOUNDARY = 1;
    public const float Y_ACTION_BOUNDARY = 1.28f;
    public const float X_FULL_BOUNDARY = 1.6f;

    // Divide pixel distances by this to reflect in game space
    public const float PIXELS_PER_UNIT = 100;

    // Volume to play sound effects
    public const float SOUND_EFFECT_VOLUME = 0.13f;

    // Number to divide previous movement by in momentum calculations
    public const float ACCELERATION_DIVISOR = 1;

    // Y value to put player at in turn-based mode
    public const float TURN_BASED_Y_POSITION = -0.45f;
    public const float BOSS_POSITION = 0.48f;

    // Ratio between UI canvas positioning and in-game positioning
    public static float CANVAS_RATIO = 3.33f;

    // Whether the game is paused or not. Signals to update methods
    private static bool paused = false;

    // Return correct velocity value that will work with any frame rate
    public static float CalculateAcceleration(float maxSpeed, float acceleration) {
        return maxSpeed / (maxSpeed / acceleration * (FRAME_RATE / 60));
    }
    // The same but uses ratio instead of acceleration value (use whichever you have the numbers for)
    public static float CalculateAccelerationWithRatio(float maxSpeed, float ratio) {
        return maxSpeed / (ratio * (FRAME_RATE / 60));
    }

    // Return a movement of the specified distance and angle
    public static Vector3 MoveAtAngle(float angle, float distance) {
        // Adjust because unity rotation is counterclockwise
        angle = 360 - angle;
        float xChange = (float) (distance * Math.Sin(angle * (Math.PI / 180))); // Convert to radians
        float yChange = (float) (distance * Math.Cos(angle * (Math.PI / 180)));

        return new Vector3(xChange, yChange, 0);
    }

    // Movement considering previous acceleration
    public static Vector3 MoveAtAngleWithMomentum(float angle, float distance, Vector3 previousMovement) {
        Vector3 result = MoveAtAngle(angle, distance);
        result += previousMovement;

        return result;
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

    // Returns a new angle a step closer to the desired angle at specified speed
    public static Quaternion TurnToAngle(float current, float target, float speed, int direction=0) {
        float result = 0;
        float distance = Math.Abs(target - current);
        if (distance < speed)
            speed = distance;

        if (direction == 0) {
            if (target < current)
                result = current - speed;
            else if (target > current)
                result = current + speed;
        } else {
            result = current + speed * direction;
        }

        return Quaternion.Euler(0, 0, result);
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

    // Movement considering previous acceleration
    public static Vector3 MoveTowardsPointWithMomentum(Vector3 currentPosition, Vector3 destination, float speed, Vector3 previousMovement) {

        Vector3 result = MoveTowardsPoint(currentPosition, destination, speed);
        result += previousMovement * 0.97f;

        return result;
    }

    // Flickers sprite alpha for length of time
    // If seconds is 0, do it forever
    public static IEnumerator FlickerSprite(SpriteRenderer s, float seconds, float flashTime=0.05f) {
        float total = 0;
        Color temp = s.color;

        // Loop for iframe time
        while ((total < seconds || seconds == 0) && s != null) {
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
    public static IEnumerator FlashSprite(SpriteRenderer s, Material m, float time=0.05f) {
        Material defaultMaterial = s.material;

        // Flash for one 20th of a second (or specified time)
        s.material = m;
        yield return new WaitForSeconds(time);
        s.material = defaultMaterial;
    }

    // Spawn a random explosion somewhere on sprite
    private static void RandomExplosion(SpriteRenderer s, GameObject explosion) {
        System.Random rand = new System.Random();
        float explosionX = ((float) rand.Next((int) (s.bounds.size.x / 2 * -1 * PIXELS_PER_UNIT),
                            (int) (s.bounds.size.x / 2 * PIXELS_PER_UNIT + 1))) / PIXELS_PER_UNIT;
        float explosionY = ((float) rand.Next((int) (s.bounds.size.y / 2 * -1 * PIXELS_PER_UNIT),
                            (int) (s.bounds.size.y / 2 * PIXELS_PER_UNIT + 1))) / PIXELS_PER_UNIT;

        Instantiate(explosion, s.gameObject.transform.position + new Vector3(explosionX, explosionY), s.gameObject.transform.rotation);
    }

    // Create unending explosions on sprite
    public static IEnumerator StartExploding(SpriteRenderer s, GameObject explosion) {
        float explodeTime = 0.2f; // 5th of a second

        while (s != null) {
            RandomExplosion(s, explosion);
            yield return new WaitForSeconds(explodeTime);
        }
    }

    // Emit a particle effect from a given point relative to a game object
    public static IEnumerator EmitParticles(GameObject source, GameObject particlePrefab, float xOffset, float yOffset, float seconds) {
        float totalTime = 0;

        while (totalTime < seconds) {
            // Find point of origin
            Vector3 point = source.transform.position + new Vector3(xOffset, yOffset, 0);

            // Spawn a particle
            Instantiate(particlePrefab, point, Quaternion.Euler(0, 0, 0));

            // Wait for a tiny bit of time
            yield return new WaitForSeconds(0.01f); // 1 100th of a second!
            totalTime += 0.01f;
        }
    }

    // Destroy gameObject after length of time
    public static IEnumerator DelayedDestroy(GameObject obj, float seconds) {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }

    // Check if gameobject is outside action mode boundaries
    public static bool OutOfBounds(GameObject obj) {
        SpriteRenderer s = obj.GetComponent<SpriteRenderer>();

        // Empty objects are considered in bounds regardless of where they are
        if (s == null)
            return false;
        
        if (obj.transform.position.y > Y_ACTION_BOUNDARY + s.bounds.extents.y * 2 || obj.transform.position.y < Y_ACTION_BOUNDARY * -1 - s.bounds.extents.y * 2)
            return true;
        if (obj.transform.position.x > X_ACTION_BOUNDARY + s.bounds.extents.x * 2 || obj.transform.position.x < X_ACTION_BOUNDARY * -1 - s.bounds.extents.x * 2)
            return true;
        
        return false;
    }

    // Plays a sound effect through given source at appropriate volume.
    // Will randomize pitch between 0 and 1.
    public static void PlaySoundEffect(AudioClip sound, AudioSource source, float pitchVar, float volume=SOUND_EFFECT_VOLUME, float pitch=1) {
        // Don't play sound if source is out of bounds
        if (OutOfBounds(source.gameObject))
            return;

        // Get random pitch in range
        System.Random r = new System.Random();
        pitchVar *= r.Next(0, 101) / 100.0f;

        source.pitch = pitch;
        if (pitchVar != 0)
            source.pitch += 1 * pitchVar * RandomSign();
        source.PlayOneShot(sound, volume);
    }

    // Randomly returns -1 or 1
    public static int RandomSign() {
        System.Random r = new System.Random();
        return -1 + 2 * r.Next(0, 2);
    }

    // Randomly returns float between 0 and 1
    public static float RandomPercentage() {
        System.Random r = new System.Random();
        return r.Next(0, 101) / 100f;
    }

    // Returns random point between two other points
    public static Vector3 RandomPoint(Vector3 point1, Vector3 point2) {
        float xDistance = Math.Abs(point2.x - point1.x) * RandomPercentage();
        float yDistance = Math.Abs(point2.y - point1.y) * RandomPercentage();

        Vector3 result = new Vector3(Math.Min(point1.x, point2.x) + xDistance, Math.Min(point1.y, point2.y) + yDistance, 0);
        return result;
    }

    // Pause/Unpause game
    public static void SetPaused(bool p) {
        paused = p;
    }

    public static bool IsPaused() {
        return paused;
    }
}
