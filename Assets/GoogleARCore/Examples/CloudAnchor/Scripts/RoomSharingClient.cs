
namespace GoogleARCore.Examples.CloudAnchor
{
    using System;
    using UnityEngine.Networking;

    /// <summary>
    /// Network Client that resolves rooms to anchor id on other devices.
    /// </summary>
    public class RoomSharingClient : NetworkClient
    {
        /// <summary>
        /// The callback to call after the anchor id is received.
        /// </summary>
        private GetAnchorIdFromRoomDelegate m_GetAnchorIdFromRoomCallback;

        /// <summary>
        /// The room id to resolve.
        /// </summary>
        private int m_RoomId;

        /// <summary>
        /// Get anchor identifier from room delegate.
        /// </summary>
        /// <param name="found">Tells if the Anchor id was found or not.</param>
        /// <param name="anchorId">The anchor id of the room.</param>
        public delegate void GetAnchorIdFromRoomDelegate(bool found, string anchorId);

        /// <summary>
        /// Gets the anchor id of a room.
        /// </summary>
        /// <param name="roomId">Room identifier to resolve.</param>
        /// <param name="ipAddress">The Ip address of the device where the room belongs to.</param>
        /// <param name="GetAnchorIdFromRoomCallback">The callback to be called after the room was resolved.</param>
        public void GetAnchorIdFromRoom(Int32 roomId, string ipAddress, GetAnchorIdFromRoomDelegate GetAnchorIdFromRoomCallback)
        {
            m_GetAnchorIdFromRoomCallback = GetAnchorIdFromRoomCallback;
            m_RoomId = roomId;
            RegisterHandler(MsgType.Connect, OnConnected);
            RegisterHandler(RoomSharingMsgType.AnchorIdFromRoomResponse, OnGetAnchorIdFromRoomResponse);
            Connect(ipAddress, 8888);
        }

        /// <summary>
        /// Handles connected event.
        /// </summary>
        /// <param name="networkMessage">The Connect response.</param>
        private void OnConnected(NetworkMessage networkMessage)
        {
            AnchorIdFromRoomRequestMessage anchorIdRequestMessage = new AnchorIdFromRoomRequestMessage
            {
                RoomId = m_RoomId
            };

            Send(RoomSharingMsgType.AnchorIdFromRoomRequest, anchorIdRequestMessage);
        }

        /// <summary>
        /// Handles the resolve room response from server.
        /// </summary>
        /// <param name="networkMessage">The resolve room response message.</param>
        private void OnGetAnchorIdFromRoomResponse(NetworkMessage networkMessage)
        {
            var response = networkMessage.ReadMessage<AnchorIdFromRoomResponseMessage>();
            m_GetAnchorIdFromRoomCallback(response.Found, response.AnchorId);
        }
    }
}