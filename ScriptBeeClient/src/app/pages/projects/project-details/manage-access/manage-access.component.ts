import { Component, computed, inject, signal } from '@angular/core';
import { ProjectStateService } from '../../../../services/projects/project-state.service';
import { ProjectService } from '../../../../services/projects/project.service';
import { ProjectMember, UserInfo } from '../../../../types/project';
import { rxResource, toObservable, toSignal } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { LoadingProgressBarComponent } from '../../../../components/loading-progress-bar/loading-progress-bar.component';
import { ErrorStateComponent } from '../../../../components/error-state/error-state.component';
import { AsyncPipe } from '@angular/common';
import { map, startWith } from 'rxjs';

type MemberType = 'user' | 'group';

@Component({
  selector: 'app-manage-access',
  templateUrl: './manage-access.component.html',
  styleUrl: './manage-access.component.scss',
  imports: [
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatAutocompleteModule,
    MatCardModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    LoadingProgressBarComponent,
    ErrorStateComponent,
    AsyncPipe,
  ],
})
export class ManageAccessComponent {
  readonly displayedColumns = ['memberId', 'memberType', 'role', 'actions'];

  private projectStateService = inject(ProjectStateService);
  private projectService = inject(ProjectService);
  private snackbar = inject(MatSnackBar);

  project = computed(() => this.projectStateService.currentProject()!);

  membersResource = rxResource({
    params: () => this.project().id,
    stream: ({ params: id }) => this.projectService.getProjectMembers(id),
  });

  usersResource = rxResource({
    stream: () => this.projectService.getAllUsers(),
  });

  memberTypeControl = new FormControl<MemberType>('user', { nonNullable: true });

  addMemberForm = new FormGroup({
    memberType: this.memberTypeControl,
    memberId: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
    role: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
  });

  memberType = toSignal(this.addMemberForm.controls.memberType.valueChanges, {
    initialValue: 'user' as MemberType,
  });

  isUserType = computed(() => this.memberType() === 'user');

  filteredUsers = toObservable(this.usersResource.value).pipe(
    startWith([] as UserInfo[]),
    map((users) => users ?? [])
  );

  isSaving = signal(false);
  removingMemberId = signal<string | null>(null);

  onUserSelected(user: UserInfo) {
    this.addMemberForm.patchValue({ memberId: user.id });
  }

  onAddMember() {
    if (this.addMemberForm.invalid) {
      return;
    }

    const { memberId, memberType, role } = this.addMemberForm.getRawValue();
    this.isSaving.set(true);

    this.projectService.updateProjectMember(this.project().id, memberId, role, memberType).subscribe({
      next: () => {
        this.membersResource.reload();
        this.addMemberForm.reset({ memberType: 'user', memberId: '', role: '' });
        this.snackbar.open('Member access updated.', 'Dismiss', { duration: 3000 });
      },
      error: () => {
        this.snackbar.open('Failed to update member access.', 'Dismiss', { duration: 4000 });
      },
      complete: () => {
        this.isSaving.set(false);
      },
    });
  }

  onRemoveMember(member: ProjectMember) {
    this.removingMemberId.set(member.memberId);

    this.projectService.removeProjectMember(this.project().id, member.memberId, member.memberType).subscribe({
      next: () => {
        this.membersResource.reload();
        this.snackbar.open('Member removed.', 'Dismiss', { duration: 3000 });
      },
      error: () => {
        this.snackbar.open('Failed to remove member.', 'Dismiss', { duration: 4000 });
      },
      complete: () => {
        this.removingMemberId.set(null);
      },
    });
  }
}
