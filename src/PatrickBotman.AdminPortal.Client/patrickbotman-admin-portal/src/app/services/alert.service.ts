import { ElementRef, Injectable, ViewChild } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private readonly alertState = new Subject<AlertParameters | undefined>();
  public alert = this.alertState.asObservable();


  constructor() { }

  showAlert(params: AlertParameters) {
    this.alertState.next(params);
  }

  closeAlert() {
    this.alertState.next(undefined);
  }
}

export interface AlertParameters {
  text: string,
  mode: 'info' | 'warning' | 'error' | 'success',
  lifetimeSeconds: number
}
