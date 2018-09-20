import { TestBed } from '@angular/core/testing';

import { BooksReadDataService } from './books-read-data.service';

describe('BooksReadDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: BooksReadDataService = TestBed.get(BooksReadDataService);
    expect(service).toBeTruthy();
  });
});
