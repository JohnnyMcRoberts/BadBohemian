import { TestBed } from '@angular/core/testing';

import { TextDataService } from './text-data.service';

describe('TextDataService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TextDataService = TestBed.get(TextDataService);
    expect(service).toBeTruthy();
  });
});
