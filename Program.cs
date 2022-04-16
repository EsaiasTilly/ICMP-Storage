using System.Net.NetworkInformation;
using System.Text;

var ping = new Ping();
var reply = ping.Send("1.1.1.1", 10000, Encoding.UTF8.GetBytes("Hello World"));
String buffer = Encoding.UTF8.GetString(reply.Buffer);
Console.WriteLine(buffer);