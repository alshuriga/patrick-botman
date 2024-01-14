import { Component, ElementRef, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [],
  templateUrl: './modal.component.html',
  styleUrl: './modal.component.scss'
})
export class ModalComponent {
  @Output() onReject = new EventEmitter();
  @Output() onSubmit = new EventEmitter();

  @Input() options! : { header: string, message: string, type: 'info' | 'error' | 'warning' };

  constructor(private elementRef: ElementRef) {

  }

  reject() {
    this.elementRef.nativeElement.remove();
    this.onReject.emit();
  }

  submit() {
    this.elementRef.nativeElement.remove();
    this.onSubmit.emit();
  }
}
