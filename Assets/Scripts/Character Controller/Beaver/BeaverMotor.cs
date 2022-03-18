using UnityEngine;

public class BeaverMotor : MonoBehaviour
{
    #region Inspector Variables

    #endregion

    #region Components
    internal Animator animator;
    #endregion

    #region Internal Variables
    internal bool isSquirming {
        get
        {
            return _isSquirming;
        }
        set
        {
            _isSquirming = value;
        }
    }
    internal bool isFarting {
        get
        {
            return _isFarting;
        }
        set
        {
            _isFarting = value;
        }
    }
    internal bool isWalking {
        get
        {
            return _IsWalking;
        }
        set
        {
            _IsWalking = value;
        }
    }
    internal bool _isSquirming;
    internal bool _isFarting;
    internal bool _IsWalking;
    #endregion

    public void Init() {
        animator = GetComponent<Animator>();

    }
}
