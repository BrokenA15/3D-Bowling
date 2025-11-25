using UnityEngine;

public class FollowHandUI : MonoBehaviour
{
    public Transform targetHand;  // la mano o un empty en la muñeca
    public Vector3 offset = new Vector3(0, 0.1f, 0.1f);

    void Update()
    {
        if (targetHand != null)
        {
            transform.position = targetHand.position + targetHand.TransformVector(offset);
            transform.rotation = Quaternion.LookRotation(
                transform.position - Camera.main.transform.position
            );
        }
    }
}
