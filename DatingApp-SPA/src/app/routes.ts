import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    {
        path: "",
        component: HomeComponent
    },
    {
        // adding dummy path for appyling guards and resolvers to multiple routes
        path: "",
        runGuardsAndResolvers: "always",
        canActivate: [AuthGuard],
        children: [
            {
                path: "members",
                component: MemberListComponent,
            },
            {
                path: "message",
                component: MessagesComponent
            },
            {
                path: "list",
                component: ListsComponent
            }
        ]
    },
    {
        path: "**",
        redirectTo: "",
        pathMatch: "full"
    }
];
