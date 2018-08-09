/* app/routes/crmRoutes.ts */
import { Request, Response, NextFunction } from "express";
import { BooksController } from "../controllers";

export class Routes {

  public booksController: BooksController = new BooksController();

  public routes(app: any): void {

    app.route('/')
      .get((req: Request, res: Response) => {
        res.status(200).send(
          {
            message: 'GET request successfulll!!!!'
          }
        );
      });

    app.route('/book')
      // Get all books
      .get(this.booksController.getBooks)

      // POST endpoint
      .post(this.booksController.addNewBook);

    // Book detail
    app.route('/book/:bookId')
      // get specific book
      .get(this.booksController.getBookById)
      .put(this.booksController.updateBook)
      .delete(this.booksController.deleteBook);


  }
}