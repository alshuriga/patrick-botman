import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatlistGridComponent } from './chatlist-grid.component';

describe('ChatlistGridComponent', () => {
  let component: ChatlistGridComponent;
  let fixture: ComponentFixture<ChatlistGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatlistGridComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChatlistGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
