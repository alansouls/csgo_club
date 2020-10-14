const { equal } = require('assert');
const http = require('http');

const hostname = '192.168.0.144';
const port = 80;

const server = http.createServer((req, res) => {
  if (req.method === "GET" && req.url === "/")
  {
    res.statusCode = 200;
    res.setHeader('Content-Type', 'text/plain');
    res.end('Server running. :)');
  }
  else fi (req.method === "GET" && req.url === "/testTunnel")
  {
      var response = http.get(`https://localhost:7071${req.url}`);
  }
});

server.listen(port, hostname, () => {
  console.log(`Server running at http://${hostname}:${port}/`);
});