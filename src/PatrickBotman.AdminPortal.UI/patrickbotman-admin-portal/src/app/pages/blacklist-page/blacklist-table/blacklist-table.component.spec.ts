import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BlacklistTableComponent } from './blacklist-table.component';

describe('BlacklistTableComponent', () => {
  let component: BlacklistTableComponent;
  let fixture: ComponentFixture<BlacklistTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BlacklistTableComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BlacklistTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
