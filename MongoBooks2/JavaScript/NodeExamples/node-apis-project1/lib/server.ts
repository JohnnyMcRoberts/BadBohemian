// lib/server.ts
import app from "./app";
const PORT = 3000;

app.listen(PORT, () => {
  console.log('let me tell you Express server listening on port ' + PORT);
})