import { ComponentFixture, TestBed } from '@angular/core/testing';

import { OnlineGifsPageComponent } from './online-gifs-page.component';

describe('OnlineGifsPageComponent', () => {
  let component: OnlineGifsPageComponent;
  let fixture: ComponentFixture<OnlineGifsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [OnlineGifsPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(OnlineGifsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
