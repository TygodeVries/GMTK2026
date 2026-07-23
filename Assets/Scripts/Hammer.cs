using UnityEngine;
using UnityEngine.InputSystem;

public class Hammer : MonoBehaviour
{
    private HingeJoint2D hammerJoint;
    [SerializeField] private float speedMultiplier = 100f;

    private void Start()
    {
        hammerJoint = GetComponent<HingeJoint2D>();
    }

    private void Update()
    {
       
    }
}
