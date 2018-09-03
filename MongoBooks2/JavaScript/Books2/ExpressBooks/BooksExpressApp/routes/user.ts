/*
 * GET users listing.
 */
import express = require('express');
const router = express.Router();

router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get users / ');
    res.send("respond with a resource");
});

export default router;