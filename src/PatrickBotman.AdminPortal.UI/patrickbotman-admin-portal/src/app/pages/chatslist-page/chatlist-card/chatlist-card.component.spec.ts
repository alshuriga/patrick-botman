import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatlistCardComponent } from './chatlist-card.component';

describe('ChatlistCardComponent', () => {
  let component: ChatlistCardComponent;
  let fixture: ComponentFixture<ChatlistCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ChatlistCardComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ChatlistCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
