import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CamerasComponent } from './cameras.component';

const routes: Routes = [{ path: '', component: CamerasComponent }];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CamerasRoutingModule { }
