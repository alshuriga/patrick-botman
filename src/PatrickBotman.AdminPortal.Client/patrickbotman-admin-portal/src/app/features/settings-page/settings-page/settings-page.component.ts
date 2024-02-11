import { Component, OnInit, ViewChild } from '@angular/core';
import { SettingsDTO } from '../../../shared/DTO';
import { GifService } from '../../../services/gif.service';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { NgIconComponent, provideIcons } from '@ng-icons/core';
import { heroXMark, heroPlus } from '@ng-icons/heroicons/outline';
import { AlertComponent } from '../../../common/alert/alert.component';
import { AlertService } from '../../../services/alert.service';
import { AdminListComponent } from '../admin-list/admin-list.component';


@Component({
  selector: 'app-settings-page',
  standalone: true,
  imports: [CommonModule, FormsModule, NgIconComponent, AlertComponent, AdminListComponent],
  templateUrl: './settings-page.component.html',
  styleUrl: './settings-page.component.scss',
  providers: [provideIcons({
    heroXMark,
    heroPlus
  })]
})


export class SettingsPageComponent implements OnInit {

  @ViewChild('form') settingsForm!: NgForm;

  settings!: SettingsDTO;
  showAlert: boolean = false;
  adminIdEntry: string = '';

  constructor(private http: GifService, private alert: AlertService) {

  }

  ngOnInit(): void {
    this.http.getSettings().subscribe(s => {
      this.settings = s;
    })
  }

  listChange() {
    this.settingsForm.form.markAsDirty();
  }
  save() {
    this.http.updateSettings(this.settings).subscribe(() => {
      this.http.getSettings().subscribe(s => {
        this.settings = s;
        this.settingsForm.form.markAsPristine();
        this.alert.showAlert({ text: 'Changes have been saved successfully.', mode: 'success', lifetimeSeconds: 3 })
      })
    })
  }
}
