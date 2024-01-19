import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, QueryList, ViewChild, ViewChildren, ViewContainerRef } from '@angular/core';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { heroXMark } from '@ng-icons/heroicons/outline';
import { AlertParameters, AlertService } from '../../services/alert.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [NgIconComponent, CommonModule],
  templateUrl: './alert.component.html',
  styleUrl: './alert.component.scss',
  providers: [provideIcons({
    heroXMark,
  })]
})
export class AlertComponent implements OnDestroy, OnInit {

  params: AlertParameters | undefined; 
  
  @ViewChild('alert') alertElement: ElementRef<HTMLDivElement> | undefined;

  transitionTime = 250;
  private sub = new Subscription();
  constructor(private alert: AlertService) {
    
  }
  
  ngOnInit(): void {
    this.sub = this.alert.alert.subscribe(val => {
      this.params = val;
      console.log(this.alertElement);
      if(this.params) {
        setTimeout(() => {
          console.log('setting opacity to 0');
          this.alertElement!.nativeElement.style['opacity'] = '0';
          this.alertElement!.nativeElement.style['transform'] = 'scale(1, 0)';
        }, (this.params?.lifetimeSeconds * 1000) - 150)
        setTimeout(() => {
          this.close()
        }, (this.params?.lifetimeSeconds * 1000))
      }
    });
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }

  show() {

  }

  close()
  {
    this.alert.closeAlert();
  }
}

