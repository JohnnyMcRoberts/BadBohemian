/*
 * GET all books listing.
 */
import express = require('express');
const router = express.Router();

var hostname: string;
router.get('/', (req: express.Request, res: express.Response) => {

  console.log('router.get allBooksHack / ');
  hostname = req.hostname;
  res.render(
    'allBooksHack',
    {
      title: 'Express all books hack',
      extra: 'some extra content -poor',
      user: hostname,
      books:
      [
          { author: "Georges Simenon", title: "Maigret is Afraid" },
          { author: "Richard House", title: "The Kills" },
          { author: "Ann Quin", title: "The Unmapped Country" }
      ]
    });
});

export default router;