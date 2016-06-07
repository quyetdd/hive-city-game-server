// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BepuPhysics.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the BepuPhysics type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models
{
    using System;

    using BEPUphysics.Character;

    using BEPUutilities;

    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Region.Models.Interfaces;

    public class BepuPhysics : IPhysics
    {
        private Position position = new Position();

        private bool dirty;

        private PlayerMovement playerMovement;

        public bool Moving { get; set; }

        public float MoveSpeed { get; set; }

        public CharacterController CharacterController { get; set; }

        public Position Position
        {
            get { return this.position; }
            set { this.position = value; }
        }

        public MoveDirection Direction { get; set; }

        public Vector3 WalkDirection { get; set; }

        public bool Dirty
        {
            get
            {
                if (CharacterController != null && CharacterController.Body != null)
                {
                    Position pos = CharacterController.Body.WorldTransform;
                    if (this.position != pos)
                    {
                        this.position = pos;
                        this.dirty = true;
                    }
                }
                return this.dirty;
            }

            set
            {
                this.dirty = value;
            }
        }

        public PlayerMovement Movement
        {
            get
            {
                return this.playerMovement;
            }

            set
            {
                this.playerMovement = value;
                if (CharacterController != null &&
                    CharacterController.Body != null)
                {
                    Moving = false;
                    Direction = MoveDirection.None;
                    //var xform = 
                    //    ((KinematicCharacterController)CharacterController).GetGhostObject().GetWorldTransform();

                    //// convert int to float 
                    //// Radian value
                    //xform.SetRotation(Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), Util.DegToRad((_playerMovement.Facing * 0.0055f))));

                    // take in an upfacing vector
                    // then rotate around the facing
                    CharacterController.Body.Orientation = Quaternion.CreateFromAxisAngle(
                                                                                          new Vector3(0, 1, 0),
                                                                                          (float)-Math.PI * (this.playerMovement.Facing * 0.0055f) / 180f);

                    Vector3 forwarddirection = CharacterController.Body.WorldTransform.Forward;
                    Vector3 updirection = CharacterController.Body.WorldTransform.Up;
                    Vector3 strafedirection = CharacterController.Body.WorldTransform.Right;

                    // make sure
                    forwarddirection.Normalize();
                    updirection.Normalize();
                    strafedirection.Normalize();

                    var moveSpeed = this.MoveSpeed;
                    this.WalkDirection = new Vector3(0, 0, 0);
                    if (this.playerMovement.Walk)
                    {
                        moveSpeed /= 4.3f;
                    }

                    if (this.playerMovement.Right < 0)
                    {
                        this.WalkDirection -= strafedirection;
                        this.Direction |= MoveDirection.Left; // 00000000 | (or) 00000100 = 00000100
                        this.Moving = true;
                    }
                    if (this.playerMovement.Right > 0)
                    {
                        this.WalkDirection += strafedirection;
                        this.Direction |= MoveDirection.Right; // 00000000 | (or) 00001000 = 00001000
                        this.Moving = true;
                    }

                    if (this.playerMovement.Forward < 0)
                    {
                        this.WalkDirection -= forwarddirection;
                        this.Direction |= MoveDirection.Backward; // 00000000 | (or) 00000010 = 00001010 = backward right
                        this.Moving = true;
                    }

                    if (this.playerMovement.Forward > 0)
                    {
                        this.WalkDirection += forwarddirection;
                        this.Direction |= MoveDirection.Forward; // 00000000 | (or) 00000001 = 00001001 = forward right
                        this.Moving = true;
                    }

                    // Character will stop after one second if client connection is dropped 
                    this.WalkDirection.Normalize();
                    Vector3 refVector = this.WalkDirection * moveSpeed;
                    CharacterController.HorizontalMotionConstraint.MovementDirection = new Vector2(refVector.X, refVector.Z);
                }
            }
        }
    }
}
