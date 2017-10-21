using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    class RecievedMessage
    {
        private TcpClient _socket;
        private User _user;
        private int _messageCode;
        private List<string> _values;

        public RecievedMessage(TcpClient socket, int messageCode)
        {
            _socket = socket;
            _messageCode = messageCode;
        }

        public RecievedMessage(TcpClient socket, int messageCode, List<string> values)
        {
            _socket = socket;
            _messageCode = messageCode;
            _values = values;
        }

        public TcpClient GetSocket() { return _socket; }
        public User GetUser() { return _user; }
        public void SetUser(User user) { _user = user; }
        public int GetMessageCode() { return _messageCode; }
        public List<string> GetValues() { return _values; }
    }
}
