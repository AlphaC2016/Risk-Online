using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    /// <summary>
    /// This class represents a message sent by a client.
    /// Fields:
    /// _socket = the TcpClient that represents the socket the message has been sent through.
    /// _user = the user that sent the message. if unknown, will be set to null.
    /// _messageCode = the message code. (big surprise there, huh?)
    /// _values = the specific vaues of the message. (values in a values field?! who would have thought!)
    /// </summary>

    class RecievedMessage
    {
        private TcpClient _socket;
        private User _user;
        private int _messageCode;
        private List<string> _values;

        /// <summary>
        /// This constructor is in case of a message without any special values.
        /// </summary>
        /// <param name="socket">the socket the message was sent in.</param>
        /// <param name="messageCode">the message code.</param>
        public RecievedMessage(TcpClient socket, int messageCode)
        {
            _socket = socket;
            _messageCode = messageCode;
        }

        /// <summary>
        /// This constructor is in case of a message with values.
        /// </summary>
        /// <param name="socket">the socket the message was sent in.</param>
        /// <param name="messageCode">the message code.</param>
        /// <param name="values">the verbose values of the message.</param>
        public RecievedMessage(TcpClient socket, int messageCode, List<string> values)
        {
            _socket = socket;
            _messageCode = messageCode;
            _values = values;
        }


        /* These functions return the values of the certain fields. */
        public TcpClient GetSocket() { return _socket; }
        public User GetUser() { return _user; }
        public void SetUser(User user) { _user = user; }
        public int GetMessageCode() { return _messageCode; }

        /// <summary>
        /// This function overrides the [] operator in order to allow direct access to the values og the message.
        /// </summary>
        /// <param name="index">the value index.</param>
        /// <returns>the specific value.</returns>
        public string this[int index]
        {
            get { return _values[index]; }
        }
    }
}
