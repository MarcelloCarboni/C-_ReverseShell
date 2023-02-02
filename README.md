# Simple C# Reverse Shell

This is a simple reverse shell written in C# to remotely execute powershell commands on the victim's machine.

If the connection fails for the client, it will try to reconnect up to 5 times (which you can of course change), waiting 30 seconds after each attempt.

To close, you can simply type `quit` to close the connection or `destroy` to delete the client executable on the victim's computer. 

It also features a server, written using C#.
If you don't want to use this server, you can simply use netcat like this `nc -lvp 1234`
