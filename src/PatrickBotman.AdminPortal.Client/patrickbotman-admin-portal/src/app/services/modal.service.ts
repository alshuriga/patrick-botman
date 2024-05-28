import { Injectable, Injector, ViewContainerRef } from '@angular/core';
import { ModalComponent } from '../common/modal/modal.component';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ModalService {

  private modalNotifier!: Subject<any>;

  constructor(private viewContainerRef: ViewContainerRef,
     private injector: Injector) {}
  open(options: { header: string, message: string, type: 'info' | 'error' | 'warning' }) {
    this.viewContainerRef.clear();
      const modalComponent = this.viewContainerRef.createComponent(ModalComponent, { injector: this.injector })
      modalComponent.instance.options = options;

      modalComponent.instance.onReject.subscribe(() => this.closeModal());
      modalComponent.instance.onSubmit.subscribe(() => this.submitModal());

      modalComponent.hostView.detectChanges();

      document.body.appendChild(modalComponent.location.nativeElement);

      this.modalNotifier = new Subject();

      return this.modalNotifier.asObservable();
  }

  closeModal() {
    this.modalNotifier.complete();
  }

  submitModal() {
    this.modalNotifier.next('confirm');
    this.closeModal();
  }
}
