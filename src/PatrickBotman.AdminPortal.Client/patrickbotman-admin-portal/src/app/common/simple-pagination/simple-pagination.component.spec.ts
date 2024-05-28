import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimplePaginationComponent } from './simple-pagination.component';

describe('SimplePaginationComponent', () => {
  let component: SimplePaginationComponent;
  let fixture: ComponentFixture<SimplePaginationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SimplePaginationComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(SimplePaginationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
