import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { heroXMark, heroPlus } from '@ng-icons/heroicons/outline';

@Component({
  selector: 'app-admin-list',
  standalone: true,
  imports: [FormsModule, CommonModule, NgIconComponent],
  templateUrl: './admin-list.component.html',
  styleUrl: './admin-list.component.scss',
  providers: [provideIcons({
    heroXMark,
    heroPlus
  })]
})
export class AdminListComponent {
  value: string = '';
  @Input() list: string[] = [];
  @Output() listChange = new EventEmitter<string[]>()
  @ViewChild('adminIdForm') settingsForm!: NgForm;

  add()
  {
    this.list.push(this.value);
    this.listChange.emit(this.list);
    this.settingsForm.form.markAsPristine();
  }

  remove(idx: number) {
    this.list.splice(idx, 1);
    this.listChange.emit(this.list);
    this.settingsForm.form.markAsPristine();
  }
}
