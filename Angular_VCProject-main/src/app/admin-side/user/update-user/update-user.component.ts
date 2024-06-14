import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgToastService } from 'ng-angular-popup';
import { Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AdminsideServiceService } from 'src/app/service/adminside-service.service';
import { AdminloginService } from 'src/app/service/adminlogin.service';

@Component({
  selector: 'app-update-user',
  templateUrl: './update-user.component.html',
  styleUrls: ['./update-user.component.css']
})
export class UpdateUserComponent implements OnInit {

  constructor(public fb: FormBuilder, private service: AdminloginService, private toastr: ToastrService, public activateRoute: ActivatedRoute, private router: Router, public toast: NgToastService) { }
  updateForm: FormGroup;
  formValid: boolean;
  userId: string; // Store the user ID
  updateData: any;

  ngOnInit(): void {
    // Initialize updateForm as an empty FormGroup instance
    this.updateForm = this.fb.group({
      id: [''],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(10)]],
      emailAddress: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(10)]],
      confirmPassword: ['', Validators.required]
    }, {validators: this.passwordCompareValidator});

    // Extract user ID from route params
    this.userId = this.activateRoute.snapshot.paramMap.get('Id');
    if (this.userId) {
      // Call method to fetch user data by ID
      this.FetchDetail(this.userId);
    }
  }

  passwordCompareValidator(fc: AbstractControl): ValidationErrors | null {
    return fc.get('password')?.value === fc.get('confirmPassword')?.value ? null : { notmatched: true }
  }
  get firstName() {
    return this.updateForm.get('firstName') as FormControl;
  }
  get lastName() {
    return this.updateForm.get('lastName') as FormControl;
  }
  get phoneNumber() {
    return this.updateForm.get('phoneNumber') as FormControl;
  }
  get emailAddress() {
    return this.updateForm.get('emailAddress') as FormControl;
  }
  get password() {
    return this.updateForm.get('password') as FormControl;
  }
  get confirmPassword() {
    return this.updateForm.get('confirmPassword') as FormControl;
  }
  // Define getters for other form controls

  FetchDetail(id: any) {
    this.service.GetUserById(id).subscribe((response: any) => {
      this.updateData = response.data.data;

      this.updateForm.patchValue({
        id: this.updateData.id,
        firstName: this.updateData.firstName,
        lastName: this.updateData.lastName,
        phoneNumber: this.updateData.phoneNumber,
        emailAddress: this.updateData.emailAddress,
        password: '',  // Password should not be populated for security reasons
        confirmPassword: '',  // Password should not be populated for security reasons
      });
    });
  }

  checkFormErrors(): string {
    let errorMessages = '';

    Object.keys(this.updateForm.controls).forEach(key => {
      const controlErrors: ValidationErrors = this.updateForm.get(key).errors;
      if (controlErrors != null) {
        Object.keys(controlErrors).forEach(keyError => {
          errorMessages += `Key control: ${key}, keyError: ${keyError}, err value: ${controlErrors[keyError]}\n`;
        });
      }
    });

    return errorMessages;
  }

  onSubmit() {
    this.checkFormErrors();
    this.formValid = true;
    if (this.updateForm.valid) {
      let updatedUserData = this.updateForm.value;
      this.service.UpdateUser(updatedUserData).subscribe((response: any) => {
        if (response.result == 1) {
          this.toast.success({ detail: "SUCCESS", summary: response.data.data, duration: 3000 });
          setTimeout(() => {
            this.router.navigate(['admin/userPage']);
          }, 1000);
        } else {
          this.toastr.error(response.data.message);
        }
      }, err => {
        this.toast.error({ detail: "ERROR", summary: err.message, duration: 3000 })
      });
      this.formValid = false;
    } else {
      this.toast.error({ detail: "ERROR", summary: this.checkFormErrors(), duration: 3000 })
    }
  }
}