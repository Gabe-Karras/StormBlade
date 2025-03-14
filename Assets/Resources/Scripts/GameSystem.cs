using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class holds important constants concerning all objects of the game.
public class GameSystem : MonoBehaviour
{
    // Frames per second!!!
    public const int FRAME_RATE = 60;

    // Divide all serialized speeds by this to get appropriate frame speed for in-game physics
    public const int SPEED_DIVISOR = 50;

    // Boundaries of action game screen
    public const float X_ACTION_BOUNDARY = 1.06f;
    public const float Y_ACTION_BOUNDARY = 1.28f;

    // Divide pixel distances by this to reflect in game space
    public const float PIXELS_PER_UNIT = 100;

    // Volume to play sound effects
    public const float SOUND_EFFECT_VOLUME = 0.2f;

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
        if (obj.transform.position.y > Y_ACTION_BOUNDARY || obj.transform.position.y < Y_ACTION_BOUNDARY * -1)
            return true;
        if (obj.transform.position.x > X_ACTION_BOUNDARY || obj.transform.position.x < X_ACTION_BOUNDARY * -1)
            return true;
        
        return false;
    }

    // Plays a sound effect through given source at appropriate volume.
    // Will randomize pitch between 0 and 1.
    public static void PlaySoundEffect(AudioClip sound, AudioSource source, float pitchVar) {
        // Don't play sound if source is out of bounds
        if (OutOfBounds(source.gameObject))
            return;

        // Get random pitch in range
        System.Random r = new System.Random();
        pitchVar *= r.Next(0, 101) / 100.0f;

        source.pitch += source.pitch * pitchVar * RandomSign();
        source.PlayOneShot(sound, SOUND_EFFECT_VOLUME);
    }

    // Randomly returns -1 or 1
    public static int RandomSign() {
        System.Random r = new System.Random();
        return -1 + 2 * r.Next(0, 2);
    }
}
