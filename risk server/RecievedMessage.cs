using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    /* This class represents a message sent by a client.
     * 
     * Fields:
     * _socket = the TcpClient that represents the socket the message has been sent through.
     * _user = the user that sent the message. if unknown, will be set to null.
     * _messageCode = the message code. (big surprise there, huh?)
     * _values = the specific vaues of the message. (values in a values field?! who would have thought!)
     */

    class RecievedMessage
    {
        private TcpClient _socket;
        private User _user;
        private int _messageCode;
        private List<string> _values;

        /* This constructor is in case of a message without any special values.*/
        public RecievedMessage(TcpClient socket, int messageCode)
        {
            _socket = socket;
            _messageCode = messageCode;
        }

        /* This constructor is in case of a message with values.*/
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

        /* This function overrides the [] operator in order to allow direct access to the values og the message.*/
        public string this[int key]
        {
            get { return _values[key]; }
        }
    }
}
