import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
registerForm!: FormGroup;
maxDate: Date;
validationErrors: string[] = [];
@Output() cancelRegister = new EventEmitter();
  constructor(private accountService: AccountService, private toast: ToastrService, 
    private fb: FormBuilder, private router: Router) { }

  ngOnInit(): void {
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
    this.initializeForm();
  }
  initializeForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['',Validators.required],
      knownAs: ['',Validators.required],
      dateOfBirth: [new Date(1970,0,1),Validators.required],
      city: ['',Validators.required],
      country: ['',Validators.required],
      password: ['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
      confirmPassword: ['',[Validators.required, this.matchValues('password')]]
    })
  }

  matchValues(matchTo: string):ValidatorFn {
    return (control:AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
      ? null: { isMatching: true}
    }
  }

  register(){
    this.accountService.register(this.registerForm.value).subscribe( response => {
      this.toast.success("Registration Successfull.");
      this.router.navigateByUrl('/members');
    }, error => {
      this.validationErrors = error;
    });
    
  }
  cancel(){
    this.cancelRegister.emit(false);
  }

}
