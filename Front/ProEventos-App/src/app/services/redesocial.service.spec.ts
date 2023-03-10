/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RedesocialService } from './redesocial.service';

describe('Service: Redesocial', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RedesocialService]
    });
  });

  it('should ...', inject([RedesocialService], (service: RedesocialService) => {
    expect(service).toBeTruthy();
  }));
});
