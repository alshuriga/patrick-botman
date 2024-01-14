import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LocalGifsPageComponent } from './local-gifs-page.component';

describe('LocalGifsPageComponent', () => {
  let component: LocalGifsPageComponent;
  let fixture: ComponentFixture<LocalGifsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LocalGifsPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(LocalGifsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
