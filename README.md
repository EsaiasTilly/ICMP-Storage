# ICMP-Storage

## Inspiration
This project was inspired by [suckerpinch](https://www.youtube.com/c/suckerpinch "suckerpinch on YouTube")'s [video](https://www.youtube.com/watch?v=JcJSW7Rprio "Harder Drive: Hard drives we didn't want or need").

## General idea
To store an arbitrary amount of data using the [ICMP echo request](https://en.wikipedia.org/wiki/Ping_(networking_utility) "ping (networking utility) on Wikipedia") the data is split into blocks. Each block is then assigned a list of IP addresses to which it will be echoed. The data is stored on the Internet by utilizing the delay between sending the data block and then receiving it.

## Write
A write request is queued which will wait for the affected blocks to be recieved and then the recieved data is updated before being echoed again.

## Read
A read request is queued which will wait for the necessary blocks to be recieved and the data is then sent to a callback. The reading of data is always performed after all writes.