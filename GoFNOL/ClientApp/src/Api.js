export class Api {
	async getUserData() {
		const response = await fetch('/api/user/data', {
			method: 'GET',
			credentials: 'same-origin'
		})
		return response.json()
	}

	async postCreateAssignmentRequest(request) {
		const requestOptions = {
			method: 'POST',
			credentials: 'same-origin',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(request)
		}

		try {
			const response = await fetch('/api/fnol', requestOptions)
			if (response.status === 200) {
				const data = await response.json()
				return { content: data }
			}
		}
		catch (e) { }

		return { error: true }
	}
}