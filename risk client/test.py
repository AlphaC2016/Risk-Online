import socket
s = socket.socket()
s.connect(("127.0.0.1", 3000))
s.send("20005alice041234")
s.recv(1024)
s.send("21304test2")
s.recv(1024)
s.recv(1024)
s.send("217")
s.recv(1024)
s.send("23405sandy")
print s.recv(1024)
print s.recv(1024)
s.recv(1024)
#is this how a comment looks like?
#guess so.
