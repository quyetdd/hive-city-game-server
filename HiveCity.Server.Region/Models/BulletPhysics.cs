//using BulletXNA.BulletDynamics;
//using BulletXNA.LinearMath;
using HiveCity.Server.Proxy.Common.MessageObjects;
using HiveCity.Server.Region.Models.Interfaces;

namespace HiveCity.Server.Region.Models
{
    //public class BulletPhysics : IPhysics
    //{
    //    private Position _position = new Position();
    //    public Vector3 WalkDirection { get; set; }
    //    private PlayerMovement _playerMovement;
    //    public bool Moving { get; set; }
    //    private bool _dirty;

    //    public float MoveSpeed { get; set; }

    //    public ICharacterControllerInterface CharacterController { get; set; }

    //    public Position Position { 
    //        get { return _position; }
    //        set { _position = value; }
    //    }

    //    public MoveDirection Direction { get; set; }

    //    public bool Dirty
    //    {
    //        get
    //        {
    //            if (CharacterController != null &&
    //                ((KinematicCharacterController)CharacterController).GetGhostObject() != null)
    //            {
    //                Position pos = ((KinematicCharacterController)CharacterController).GetGhostObject().GetWorldTransform();
    //                if (_position != pos)
    //                {
    //                    _position = pos;
    //                    _dirty = true;
    //                }
    //            }
    //            return _dirty;
    //        }
    //        set { _dirty = value; }
    //    }

    //    public PlayerMovement Movement
    //    {
    //        get { return _playerMovement; }
    //        set
    //        {
    //            _playerMovement = value;
    //            if (CharacterController != null &&
    //                ((KinematicCharacterController)CharacterController).GetGhostObject() != null)
    //            {
    //                Moving = false;
    //                Direction = MoveDirection.None;
    //                var xform =
    //                    ((KinematicCharacterController)CharacterController).GetGhostObject().GetWorldTransform();

    //                // convert int to float 
    //                // Radian value
    //                xform.SetRotation(Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), Util.DegToRad((_playerMovement.Facing * 0.0055f))));

    //                Vector3 forwardDir = xform._basis[2];
    //                Vector3 upDir = xform._basis[1];
    //                Vector3 strafeDir = xform._basis[0];
    //                forwardDir.Normalize();
    //                upDir.Normalize();
    //                strafeDir.Normalize();

    //                var moveSpeed = MoveSpeed;
    //                WalkDirection = new Vector3(0, 0, 0);
    //                if (_playerMovement.Walk)
    //                {
    //                    moveSpeed /= 4.3f;
    //                }
             
    //                if (_playerMovement.Right < 0)
    //                {
    //                    WalkDirection -= strafeDir;
    //                    Direction |= MoveDirection.Left; // 00000000 | (or) 00000100 = 00000100
    //                    Moving = true;
    //                }
    //                if (_playerMovement.Right > 0)
    //                {
    //                    WalkDirection += strafeDir;
    //                    Direction |= MoveDirection.Right; // 00000000 | (or) 00001000 = 00001000
    //                    Moving = true;
    //                }

    //                if (_playerMovement.Forward < 0)
    //                {
    //                    WalkDirection -= forwardDir;
    //                    Direction |= MoveDirection.Backward; // 00000000 | (or) 00000010 = 00001010 = backward right
    //                    Moving = true;
    //                }
    //                if (_playerMovement.Forward > 0)
    //                {
    //                    WalkDirection += forwardDir;
    //                    Direction |= MoveDirection.Forward; // 00000000 | (or) 00000001 = 00001001 = forward right
    //                    Moving = true;
    //                }

    //                // Character will stop after one second if client connection is dropped 
    //                WalkDirection.Normalize();
    //                Vector3 refVector = (WalkDirection * moveSpeed) / 30f;
    //                CharacterController.SetWalkDirection(ref refVector);
    //            }
    //        }
    //    }
    //}
}
