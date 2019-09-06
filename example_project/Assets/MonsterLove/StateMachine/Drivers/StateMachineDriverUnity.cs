using System;
using UnityEngine;

namespace MonsterLove.StateMachine
{
    public class StateMachineDriverUnity
    {
        public Action Awake;
        public Action LateUpdate;
        public Action<int> OnAnimatorIK;
        public Action OnAnimatorMove;
        public Action<bool> OnApplicationFocus;
        public Action OnApplicationPause;
        public Action OnApplicationQuit;
        public Action<float[], int> OnAudioFilterRead;
        public Action OnBecameInvisible;
        public Action OnBecameVisible;
        public Action<Collider> OnCollisionEnter;
        public Action<Collision2D> OnCollisionEnter2D;
        public Action<Collider> OnCollisionExit;
        public Action<Collider2D> OnCollisionExit2D;
        public Action<Collision> OnCollisionStay;
        public Action<Collision2D> OnCollisionStay2D;
        public Action OnConnectedToServer;
        public Action<ControllerColliderHit> OnControllerColliderHit;
        public Action OnDestroy;
        public Action OnDisable;
        public Action OnDrawGizmos;
        public Action OnDrawGizmosSelected;
        public Action OnEnable;
        public Action OnGUI;
        public Action<float> OnJointBreak;
        public Action<Joint2D> OnJointBreak2D;
        public Action OnMouseDown;
        public Action OnMouseDrag;
        public Action OnMouseEnter;
        public Action OnMouseExit;
        public Action OnMouseOver;
        public Action OnMouseUp;
        public Action OnMouseUpAsButton;
        public Action<GameObject> OnParticleCollision;
        public Action OnParticleSystemStopped;
        public Action OnParticleTrigger;
        public Action OnPostRender;
        public Action OnPreCull;
        public Action<RenderTexture, RenderTexture> OnRenderImage;
        public Action OnRenderObject;
        public Action OnTransformChildrenChanged;
        public Action OnTransformParentChanged;
        public Action<Collider> OnTriggerEnter;
        public Action<Collider2D> OnTriggerEnter2D;
        public Action<Collider> OnTriggerExit;
        public Action<Collider2D> OnTriggerExit2D;
        public Action<Collider> OnTriggerStay;
        public Action<Collider2D> OnTriggerStay2D;
        public Action OnValidate;
        public Action OnWillRenderOjbect;
        public Action Reset;
        public Action Start;
        public Action Update;

        //Unity Networking Deprecated
        //public Action<NetworkDisconnection> OnDisconnectedFromServer;
        //public Action<NetworkConnectionError> OnFailedToConnect;
        //public Action<NetworkConnectionError> OnFailedToConnectToMasterServer;
        //public Action<MasterServerEvent> OnMasterServerEvent;
        //public Action<NetworkMessageInfo> OnNetworkInstantiate;
        //public Action<NetworkPlayer> OnPlayerConnected;
        //public Action<NetworkPlayer> OnPlayerDisconnected;
        //public Action<BitStream, NetworkMessageInfo> OnSerializeNetworkView;
        //public Action OnSeverInitialized;

    }
}